using System.Diagnostics.CodeAnalysis;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.FileStorage.Infrastructures;
using Pinterest.Application.FileStorage.Infrastructures.Interfaces;
using Pinterest.Application.FileStorage.Infrastructures.Models;
using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Application.FileStorage.Models;

// ReSharper disable AccessToDisposedClosure

namespace Pinterest.S3Storage.Minio.Infrastructures;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class FileManager(IAmazonS3 s3Client, ILogger<FileManager> logger) : IFileManager
{
    private readonly IAmazonS3 _s3Client = s3Client;
    protected ILogger<FileManager> Logger { get; } = logger;

    public async Task<FileMetadata> UploadFile(Stream fileStream, StoringFileInfo fileInfo)
    {
        await _s3Client.PutObjectAsync(new PutObjectRequest()
        {
            BucketName = fileInfo.Bucket,
            Key = fileInfo.FileName,
            InputStream = fileStream
        });
        var response = await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest()
        {
            BucketName = fileInfo.Bucket,
            Key = fileInfo.FileName
        });
        return new FileMetadata(response.ContentLength, response.Headers["Content-Type"]);
    }
    public virtual async Task<string> InitiateMultipartUpload(StoringFileInfo fileInfo)
    {
        var response = await _s3Client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest()
        {
            BucketName = fileInfo.Bucket,
            Key = fileInfo.FileName
        });
        Logger.LogInformation($"Started multipart upload: {fileInfo.Bucket}/{fileInfo.FileName}");
        return response.UploadId;
    }
    public virtual async Task<UploadDataResult> UploadPartAsync(UploadData chuckInfo)
    {
        try {
            var response = await _s3Client.UploadPartAsync(new UploadPartRequest
            {
                BucketName = chuckInfo.FileInfo.Bucket,
                Key = chuckInfo.FileInfo.FileName,
                UploadId = chuckInfo.UploadId,
                PartNumber = chuckInfo.PartNumber,
                InputStream = chuckInfo.ChuckData
            });
            Logger.LogInformation($"Upload part of file: {chuckInfo.FileInfo.FileName}, Size: {chuckInfo.ChuckData.Length}");
            return new UploadDataResult(ETag: response.ETag);
        }
        catch (AmazonS3Exception error)
        {
            Logger.LogError($"Failed to initiate multipart upload: {error.Message}");
            throw new ProcessException($"Failed to initiate multipart upload: {chuckInfo.FileInfo.FileName}");
        }
    }
    public virtual async Task RemoveObjectFromStorage(StoringFileInfo fileInfo)
    {
        try {
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest()
            {
                BucketName = fileInfo.Bucket,
                Key = fileInfo.FileName
            });
        }
        catch (AmazonS3Exception error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            Logger.LogError($"File {fileInfo.Bucket}/{fileInfo.FileName} not found. Message: '{error.Message}'");
            throw new ProcessException($"File {fileInfo.Bucket}/{fileInfo.FileName} not found");
        }
        Logger.LogInformation($"Successfully removed from storage: {fileInfo.Bucket}/{fileInfo.FileName}");
    }
    public virtual async Task<FileMetadata?> CompleteMultipartUpload(CompleteUploadData completeInfo)
    {
        try {
            await _s3Client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
            {
                BucketName = completeInfo.FileInfo.Bucket,
                Key = completeInfo.FileInfo.FileName,
                UploadId = completeInfo.UploadId,
                PartETags = completeInfo.Parts.Select(part => new PartETag(part.PartNumber, part.ETag)).ToList()
            });
        }
        catch (AmazonS3Exception error) when (error.ErrorCode == "InvalidPart")
        {
            Logger.LogError($"ETags were invalid: {completeInfo.UploadId}");
            Logger.LogError($"Error message: {error.Message}");
            
            await RejectMultipartUpload(completeInfo.UploadId, completeInfo.FileInfo);
            throw new ProcessException($"ETags were invalid: {completeInfo.UploadId}");
        }
        Logger.LogInformation($"Completed multipart upload: {completeInfo.FileInfo.FileName}");
        var response = await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest()
        {
            BucketName = completeInfo.FileInfo.Bucket,
            Key = completeInfo.FileInfo.FileName
        });
        return new FileMetadata(response.ContentLength, response.Headers["Content-Type"]);
    }
    public virtual async Task RejectMultipartUpload(string uploadId, StoringFileInfo fileInfo)
    {
        try {
            await _s3Client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
            {
                BucketName = fileInfo.Bucket,
                Key = fileInfo.FileName,
                UploadId = uploadId
            });
        }
        catch (AmazonS3Exception error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            Logger.LogError($"Error encountered on server. Message: '{error.Message}'");
            throw new ProcessException($"Cannot reject multipart upload: {uploadId}");
        }
        Logger.LogInformation($"Rejected multipart upload: {fileInfo.FileName}");
    }
    public async Task<FileMetadata> GetFileMetadata(StoringFileInfo fileInfo)
    {
        try {
            var response = await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest()
            {
                BucketName = fileInfo.Bucket,
                Key = fileInfo.FileName
            });
            return new FileMetadata(response.ContentLength, response.Headers["Content-Type"]);
        }
        catch (AmazonS3Exception error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            Logger.LogError($"File {fileInfo.FileName} not found. Message: '{error.Message}'");
            throw new ProcessException($"File {fileInfo.Bucket}/{fileInfo.FileName} not found");
        }
    }
    public async Task<string> GetFileUrl(StoringFileInfo fileInfo)
    {
        try {
            var presignedUrl = await _s3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest()
            {
                BucketName = fileInfo.Bucket,
                Key = fileInfo.FileName,
                Expires = DateTime.UtcNow.AddMinutes(5),
                Verb = HttpVerb.GET,
            });
            return presignedUrl.Replace("https://", "http://");
        }
        catch (AmazonS3Exception error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            Logger.LogError($"File {fileInfo.FileName} not found. Message: '{error.Message}'");
            throw new ProcessException($"File {fileInfo.Bucket}/{fileInfo.FileName} not found");
        }
    }
    public async Task<Stream> GetFileStream(StoringFileInfo fileInfo, FileRangeInfo? fileRangeInfo)
    {
        try {
            if (fileRangeInfo != null)
            {
                var response = await _s3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = fileInfo.Bucket,
                    Key =  fileInfo.FileName,
                    ByteRange = new ByteRange(fileRangeInfo.Start, fileRangeInfo.End)
                });
                return response.ResponseStream;
            }
            return await _s3Client.GetObjectStreamAsync(fileInfo.Bucket, fileInfo.FileName, new Dictionary<string, object>());
        }
        catch (AmazonS3Exception error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            Logger.LogError($"File {fileInfo.FileName} not found. Message: '{error.Message}'");
            throw new ProcessException($"File {fileInfo.Bucket}/{fileInfo.FileName} not found");
        }
    }
}