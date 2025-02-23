using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.FileStorage.Infrastructures.Interfaces;
using Pinterest.Application.FileStorage.Infrastructures.Models;
using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Application.FileStorage.Models;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.FileStorage.Entities;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.FileStorage.Services;

internal class ThumbnailService : IThumbnailService
{
    private readonly IDocumentRepository<StorageEntity> _repository;
    private readonly IMapper _mapper;
    private readonly IModelValidator<FileRangeInfo> _validator;
    private readonly IVideoProcessing _videoProcessing;
    private readonly IFileManager _fileManager;

    public ThumbnailService(IDocumentRepository<StorageEntity> repository,
        IMapper mapper,
        IModelValidator<FileRangeInfo> validator,
        IVideoProcessing videoProcessing,
        IFileManager fileManager,
        ILogger<ThumbnailService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
        _videoProcessing = videoProcessing;
        _fileManager = fileManager;
        Logger = logger;
    }
    private ILogger<ThumbnailService> Logger { get; }
    private async Task<StorageEntity> GetFileEntity(Guid fileUuid)
    {
        var fileInfo = await _repository.Collection.Find(item => item.FileUuid == fileUuid).FirstOrDefaultAsync();
        ProcessException.ThrowIf(() => fileInfo == null, $"File {fileUuid} not found");
        return fileInfo;
    }
    public async Task ProcessFileThumbnail(Guid fileUuid)
    {
        var storageEntity = await GetFileEntity(fileUuid);
        if (storageEntity.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
        {
            var fileUrl = await _fileManager.GetFileUrl(_mapper.Map<StoringFileInfo>(storageEntity));
            await using var thumbnailStream = await _videoProcessing.GetVideoThumbnail(fileUrl);

            var thumbnailKey = $"{Guid.NewGuid()}_thumbnail.jpg";
            await _fileManager.UploadFile(thumbnailStream, new StoringFileInfo()
            {
                Bucket = storageEntity.BucketName,
                FileName = thumbnailKey
            });
            var updater = _repository.UpdateBuilder.Set(item => item.Thumbnail, new ThumbnailFile()
            {
                FileName = thumbnailKey
            });
            await _repository.Collection.UpdateOneAsync(item => item.FileUuid == storageEntity.FileUuid, updater);
        }
    }
    public async Task RemoveThumbnail(Guid fileUuid)
    {
        var storageEntity = await GetFileEntity(fileUuid);
        if (storageEntity.Thumbnail != null)
        {
            await _fileManager.RemoveObjectFromStorage(new StoringFileInfo()
            {
                Bucket = storageEntity.BucketName,
                FileName = storageEntity.Thumbnail.FileName
            });
        }
    }
    public async Task<Stream> GetFileThumbnail(Guid fileUuid, FileRangeInfo? rangeInfo = default)
    {
        if (rangeInfo != null) await _validator.CheckAsync(rangeInfo);

        var storageEntity = await GetFileEntity(fileUuid);
        if (storageEntity.Thumbnail == null) throw new ProcessException($"File {fileUuid} not have thumbnail yet");
        
        return await _fileManager.GetFileStream(new StoringFileInfo()
        {
            Bucket = storageEntity.BucketName,
            FileName = storageEntity.Thumbnail.FileName
        }, rangeInfo);
    }
    public async Task<FileMetadata> GetThumbnailMetadata(Guid fileUuid)
    {
        var storageEntity = await GetFileEntity(fileUuid);
        if (storageEntity.Thumbnail == null) throw new ProcessException($"File {fileUuid} not have thumbnail yet");
        return await _fileManager.GetFileMetadata(new StoringFileInfo()
        {
            Bucket = storageEntity.BucketName,
            FileName = storageEntity.Thumbnail.FileName
        });
    }
}