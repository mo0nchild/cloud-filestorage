using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.FileStorage.Infrastructures;
using Pinterest.Application.FileStorage.Infrastructures.Interfaces;
using Pinterest.Application.FileStorage.Infrastructures.Models;
using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Application.FileStorage.Models;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.FileStorage.Entities;
using Pinterest.Domain.FileStorage.Settings;
using Pinterest.Domain.Messages.FileStorageMessages;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.FileStorage.Services;

internal class FileStorageService : IFileStorageService
{
    private readonly IDocumentRepository<StorageEntity> _repository;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _producer;
    private readonly IFileStorageValidators _validators;
    private readonly IFileManager _fileManager;

    public FileStorageService(IDocumentRepository<StorageEntity> repository, 
        IFileManager fileManager,
        IMapper mapper,
        IFileStorageValidators validators,
        IMessageProducer producer,
        IOptions<StorageSettings> options,
        ILogger<FileStorageService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _producer = producer;
        _validators = validators;
        _fileManager = fileManager;
        (Logger, Settings) = (logger, options.Value);
    }
    private ILogger<FileStorageService> Logger { get; }
    private StorageSettings Settings { get; }

    private bool TryGetBucketByFileName(string fileName, out (BucketByExtension? Bucket, string Extension) result)
    {
        result.Extension = Path.GetExtension(fileName);
        result.Bucket = Settings.BucketByExtensions
            .FirstOrDefault(item => item.Extensions.Any(it => it == Path.GetExtension(fileName)));
        return result.Bucket != null;
    }
    private async Task<StorageEntity> GetFileEntity(Guid fileUuid)
    {
        var fileInfo = await _repository.Collection.Find(item => item.FileUuid == fileUuid).FirstOrDefaultAsync();
        ProcessException.ThrowIf(() => fileInfo == null, $"File {fileUuid} not found");
        return fileInfo;
    }
    public async Task<Guid> UploadFile(NewFileInfo fileInfo, Stream fileStream)
    {
        var bucketExists = TryGetBucketByFileName(fileInfo.FileName, out var bucketByExtension);
        ProcessException.ThrowIf(() => !bucketExists, $"File not supported: {fileInfo.FileName}");
        
        await _validators.NewFileValidator.CheckAsync(fileInfo);
        ProcessException.ThrowIf(() => fileStream.Length <= 0, $"File {fileInfo.FileName} size is zero");
        
        var storingFileName = $"{Guid.NewGuid()}.{bucketByExtension.Extension}";
        var result = await _fileManager.UploadFile(fileStream, new StoringFileInfo()
        {
            FileName = storingFileName,
            Bucket = bucketByExtension.Bucket!.BucketName
        });
        var fileUuid = Guid.NewGuid();
        await _repository.Collection.InsertOneAsync(new StorageEntity()
        {
            FileUuid = fileUuid,
            BucketName = bucketByExtension.Bucket!.BucketName,
            FileName = storingFileName,
            OriginalName = fileInfo.FileName,
            UploadId = string.Empty,
            IsUploaded = true
        });
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
        var fileUuid = Guid.NewGuid();
        await _repository.Collection.InsertOneAsync(new StorageEntity()
        {
            FileUuid = fileUuid,
            BucketName = bucketByExtension.Bucket!.BucketName,
            FileName = storingFileName,
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
        return new PartInfo()
        {
            ETag = uploadResult.ETag,
            PartNumber = chuckInfo.PartNumber,
        };
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
                _repository.UpdateBuilder.Set(item => item.FileSize, result.FileSize)
            });
            await _repository.Collection.UpdateOneAsync(item => item.FileUuid == completeInfo.FileUuid, updateDef);
        }
        else
        {
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
    public async Task<FileMetadata> GetFileMetadata(Guid fileUuid)
    {
        var fileInfo = await GetFileEntity(fileUuid);
        return await _fileManager.GetFileMetadata(_mapper.Map<StoringFileInfo>(fileInfo));
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