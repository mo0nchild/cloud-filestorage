using AutoMapper;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.FileStorage.Infrastructures;
using CloudStorage.Application.FileStorage.Infrastructures.Interfaces;
using CloudStorage.Application.FileStorage.Infrastructures.Models;
using CloudStorage.Application.FileStorage.Interfaces;
using CloudStorage.Application.FileStorage.Models;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Core.Repositories;
using CloudStorage.Domain.FileStorage.Entities;
using CloudStorage.Domain.FileStorage.Settings;
using CloudStorage.Domain.Messages.FileStorageMessages;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.FileStorage.Services;

internal class FileStorageService : IFileStorageService
{
    private readonly IDocumentRepository<StorageEntity> _repository;
    private readonly IMapper _mapper;
    private readonly IThumbnailService _thumbnailService;
    private readonly IMessageProducer _producer;
    private readonly IFileStorageValidators _validators;
    private readonly IFileManager _fileManager;
    
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new(); 
    public FileStorageService(IDocumentRepository<StorageEntity> repository, 
        IFileManager fileManager,
        IMapper mapper,
        IThumbnailService thumbnailService,
        IFileStorageValidators validators,
        IMessageProducer producer,
        IOptions<StorageSettings> options,
        ILogger<FileStorageService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _thumbnailService = thumbnailService;
        _producer = producer;
        _validators = validators;
        _fileManager = fileManager;
        (Logger, Settings) = (logger, options.Value);
    }
    private ILogger<FileStorageService> Logger { get; }
    private StorageSettings Settings { get; }

    private record NewStoragingFileInfo(string BucketName, string Extension, string? FileName = null);

    private bool TryGetBucketByFileName(string fileName, out (BucketByExtension? Bucket, string Extension) result)
    {
        result.Extension = Path.GetExtension(fileName);
        result.Bucket = Settings.BucketByExtensions
            .FirstOrDefault(item => item.Extensions.Any(it => it == Path.GetExtension(fileName)));
        return result.Bucket != null;
    }
    private async Task<Guid> ProcessUploadFullFile(NewStoragingFileInfo fileInfo, Stream fileStream)
    {
        var storingFileName = $"{Guid.NewGuid()}.{fileInfo.Extension}";
        var result = await _fileManager.UploadFile(fileStream, new StoringFileInfo()
        {
            FileName = storingFileName,
            Bucket = fileInfo.BucketName
        });
        var fileUuid = Guid.NewGuid();
        await _repository.Collection.InsertOneAsync(new StorageEntity()
        {
            FileUuid = fileUuid,
            BucketName = fileInfo.BucketName,
            FileName = storingFileName,
            ContentType = result.ContentType,
            FileSize = result.FileSize,
            OriginalName = fileInfo.FileName ?? string.Empty,
            UploadId = string.Empty,
            IsUploaded = true
        });
        return fileUuid;
    }
    private async Task<StorageEntity> GetFileEntity(Guid fileUuid)
    {
        var fileInfo = await _repository.Collection.Find(item => item.FileUuid == fileUuid).FirstOrDefaultAsync();
        ProcessException.ThrowIf(() => fileInfo == null, $"File {fileUuid} not found");
        return fileInfo;
    }
    public async Task<Guid> UploadFile(NewFileInfo fileInfo, Stream fileStream)
    {
        await _validators.NewFileValidator.CheckAsync(fileInfo);
        var bucketExists = TryGetBucketByFileName(fileInfo.FileName, out var bucketByExtension);
        
        ProcessException.ThrowIf(() => !bucketExists, $"File not supported: {fileInfo.FileName}");
        ProcessException.ThrowIf(() => fileStream.Length <= 0, $"File {fileInfo.FileName} size is zero");

        var newFileInfo = new NewStoragingFileInfo(bucketByExtension.Bucket!.BucketName, bucketByExtension.Extension,
            FileName: fileInfo.FileName);
        var fileUuid = await ProcessUploadFullFile(newFileInfo, fileStream);
        
        await _thumbnailService.ProcessFileThumbnail(fileUuid);
        return fileUuid;
    }
    public async Task<Guid> InitializeUpload(NewFileInfo fileInfo)
    {
        var bucketExists = TryGetBucketByFileName(fileInfo.FileName, out var bucketByExtension);
        ProcessException.ThrowIf(() => !bucketExists, $"File not supported: {fileInfo.FileName}");
     
        await _validators.NewFileValidator.CheckAsync(fileInfo);
        var storingFileName = $"{Guid.NewGuid()}.{bucketByExtension.Extension}";
        var uploadId = await _fileManager.InitiateMultipartUpload(new StoringFileInfo()
        {
            FileName = storingFileName,
            Bucket = bucketByExtension.Bucket!.BucketName
        });
        if (!_contentTypeProvider.TryGetContentType(fileInfo.FileName, out var contentType))
        {
            Logger.LogError($"File {fileInfo.FileName} not supported: Not supported content type");
            throw new ProcessException($"Content type not supported: {fileInfo.FileName}");
        }
        var fileUuid = Guid.NewGuid();
        await _repository.Collection.InsertOneAsync(new StorageEntity()
        {
            FileUuid = fileUuid,
            BucketName = bucketByExtension.Bucket!.BucketName,
            FileName = storingFileName,
            ContentType = contentType,
            OriginalName = fileInfo.FileName,
            UploadId = uploadId
        });
        return fileUuid;
    }
    public async Task<PartInfo> UploadFilePart(UploadChuckInfo chuckInfo)
    {
        await _validators.UploadChuckValidator.CheckAsync(chuckInfo);
        var fileInfo = await GetFileEntity(chuckInfo.FileUuid);
        var uploadResult = await _fileManager.UploadPartAsync(new UploadData()
        {
            ChuckData = chuckInfo.ChuckData,
            PartNumber = chuckInfo.PartNumber,
            UploadId = fileInfo.UploadId,
            FileInfo = _mapper.Map<StoringFileInfo>(fileInfo),
        });
        return new PartInfo() { ETag = uploadResult.ETag, PartNumber = chuckInfo.PartNumber, };
    }
    public async Task CompleteUpload(CompleteUploadInfo completeInfo)
    {
        await _validators.CompleteUploadValidator.CheckAsync(completeInfo);
        
        var fileInfo = await GetFileEntity(completeInfo.FileUuid);
        var result = await _fileManager.CompleteMultipartUpload(new CompleteUploadData()
        {
            Parts = completeInfo.Parts,
            UploadId = fileInfo!.UploadId,
            FileInfo = _mapper.Map<StoringFileInfo>(fileInfo),
        });
        if (result != null)
        {
            var updateDef = _repository.UpdateBuilder.Combine(new []
            {
                _repository.UpdateBuilder.Set(item => item.IsUploaded, true),
                _repository.UpdateBuilder.Set(item => item.FileSize, result.FileSize),
                _repository.UpdateBuilder.Set(item => item.ContentType, result.ContentType)
            });
            await _repository.Collection.UpdateOneAsync(item => item.FileUuid == completeInfo.FileUuid, updateDef);
            await _thumbnailService.ProcessFileThumbnail(fileInfo.FileUuid);
        }
        else {
            await _repository.Collection.DeleteOneAsync(item => item.FileUuid == completeInfo.FileUuid);
            throw new ProcessException("Complete upload error, ETags list is invalid");
        }
    }
    public async Task DeleteFile(Guid fileUuid)
    {
        var fileInfo = await GetFileEntity(fileUuid);
        if (!fileInfo.IsUploaded)
        {
            await _fileManager.RejectMultipartUpload(fileInfo.UploadId, _mapper.Map<StoringFileInfo>(fileInfo));
        }
        else await _fileManager.RemoveObjectFromStorage(_mapper.Map<StoringFileInfo>(fileInfo));
        await _thumbnailService.RemoveThumbnail(fileInfo.FileUuid);
        
        await _repository.Collection.DeleteOneAsync(item => item.FileUuid == fileUuid);
        await _producer.SendToAllAsync(FileRemovedMessage.RoutingPath, new FileRemovedMessage()
        {
            FileUuid = fileInfo.FileUuid,
        });
    }
    public async Task SetFileIsUsing(Guid fileUuid)
    {
        if (await _repository.Collection.Find(item => item.FileUuid == fileUuid).CountDocumentsAsync() > 0)
        {
            var updateDef = _repository.UpdateBuilder.Set(item => item.IsUsing, true);
            await _repository.Collection.UpdateOneAsync(item => item.FileUuid == fileUuid, updateDef);
        }
        else throw new ProcessException($"File {fileUuid} does not exist");
    }
    public async Task<FileBasicInfo> GetFileMetadata(Guid fileUuid)
    {
        return _mapper.Map<FileBasicInfo>(await GetFileEntity(fileUuid));
    }
    public async Task<Stream> GetFileData(Guid fileUuid, FileRangeInfo? rangeInfo = default)
    {
        if (rangeInfo != null) await _validators.FileRangeValidator.CheckAsync(rangeInfo);
        
        var fileInfo = await GetFileEntity(fileUuid);
        return await _fileManager.GetFileStream(_mapper.Map<StoringFileInfo>(fileInfo), rangeInfo);
    }
    public async Task RemoveNotUsingFiles()
    {
        var notUsingFiles = await _repository.Collection.Find(item => !item.IsUsing).ToListAsync();
        foreach (var item in notUsingFiles)
        {
            var ttlLimit = item.IsUploaded ? Settings.NotUsingTtl : Settings.NotCompletedTtl;
            if ((DateTime.UtcNow - item.CreatedTime).TotalMilliseconds < ttlLimit) continue;
            await DeleteFile(item.FileUuid);
        }
    }
}