using System.Collections.Immutable;
using System.Net;
using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.FileStorage.Infrastructures;
using CloudStorage.Application.FileStorage.Infrastructures.Interfaces;
using BucketAlreadyExistsException = Amazon.S3.Model.BucketAlreadyExistsException;

namespace CloudStorage.S3Storage.Minio.Infrastructures;

internal class BucketManager(IAmazonS3 s3Client, ILogger<FileManager> logger) : IBucketManager
{
    private readonly IAmazonS3 _s3Client = s3Client;

    private ILogger<FileManager> Logger { get; } = logger;
    public virtual async Task CreateBucketAsync(string bucketName)
    {
        if (!await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName))
        {
            await _s3Client.EnsureBucketExistsAsync(bucketName);
        }
    }
    public virtual async Task DeleteBucketAsync(string bucketName)
    {
        if (await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName))
        {
            await _s3Client.DeleteBucketAsync(bucketName);
        }
    }
    public virtual async Task<int> GetFilesInBucketCount(string bucketName)
    {
        var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        return bucketExists ? (await _s3Client.ListObjectsAsync(bucketName)).S3Objects.Count : -1;
    }
    public async Task<IReadOnlyList<string>> GetBucketsListAsync()
    {
        return (await _s3Client.ListBucketsAsync()).Buckets.Select(item => item.BucketName).ToList();
    }
}