using System.Diagnostics.CodeAnalysis;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Commons.S3Storage;
using BucketInfo = Pinterest.Application.Commons.S3Storage.BucketInfo;

// ReSharper disable AccessToDisposedClosure

namespace Pinterest.S3Storage.Minio.Infrastructures;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class S3StorageService(IAmazonS3 s3Client, ILogger<S3StorageService> logger)
    : IS3StorageService
{
    private readonly IAmazonS3 _s3Client = s3Client;
    protected ILogger<S3StorageService> Logger { get; } = logger;

    public virtual async Task<string?> GetObjectUrlFromStorage(BucketInfo info, DateTime expiry)
    {
        try
        {
            var presignedUrl = await _s3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest()
            {
                BucketName = info.BucketName,
                Expires = expiry,
                Key = info.ObjectName,
                Protocol = Protocol.HTTP,
            });
            Logger.LogInformation($"Object presigned URL: ${presignedUrl}");
            return presignedUrl;
        }
        catch (AmazonS3Exception error)
        {
            Logger.LogError($"Error encountered on server. Message: '{error.Message}'");
            return null;
        }
    }
    public virtual async Task<byte[]?> GetObjectFromStorage(BucketInfo info)
    {
        using var memoryStream = new MemoryStream();
        try
        {
            var response = await _s3Client.GetObjectAsync(new GetObjectRequest()
            {
                BucketName = info.BucketName,
                Key = info.ObjectName,
            });
            await response.ResponseStream.CopyToAsync(memoryStream);
        }
        catch (AmazonS3Exception error)
        {
            Logger.LogError($"Error encountered on server. Message: '{error.Message}'");
            return null;
        }
        return memoryStream.ToArray();
    }
    public virtual async Task<bool> LoadObjectToStorage(byte[] file, BucketInfo info)
    {
        using var requestData = new MemoryStream(file);
        requestData.Seek(0, SeekOrigin.Begin);
        try
        {
            await _s3Client.PutObjectAsync(new PutObjectRequest()
            {
                BucketName = info.BucketName,
                Key = info.ObjectName,
                InputStream = requestData,
                ContentType = "application/octet-stream"
            });
        }
        catch (AmazonS3Exception error)
        {
            Logger.LogError($"Error encountered on server. Message: '{error.Message}'");
            return false;
        }
        Logger.LogInformation($"Successfully uploaded to storage: {info.ObjectName}");
        return true;
    }
    public virtual async Task<bool> RemoveObjectFromStorage(BucketInfo info)
    {
        try
        {
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest()
            {
                BucketName = info.BucketName,
                Key = info.ObjectName
            });
        }
        catch (AmazonS3Exception error)
        {
            Logger.LogError($"Error encountered on server. Message: '{error.Message}'");
            return false;
        }
        Logger.LogInformation($"Successfully removed from storage: {info.ObjectName}");
        return true;
    }
}