using System.Net;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Commons.S3Storage;
using Pinterest.S3Storage.Minio.Infrastructures;
using Pinterest.S3Storage.Minio.Settings;
// ReSharper disable ConvertToConstant.Local
// ReSharper disable ConvertToLambdaExpression

namespace Pinterest.S3Storage.Minio;

public static class Bootstrapper
{
    private static readonly string S3StorageSection = "S3Storage";
    
    public static async Task<IServiceCollection> AddS3StorageService(this IServiceCollection collection,
        IConfiguration configuration)
    {
        var options = configuration.GetSection(S3StorageSection).Get<S3StorageOptions>();
        if (options == null) throw new Exception($"Not found {S3StorageSection} options in configuration");
        collection.AddSingleton<IAmazonS3, AmazonS3Client>(provider =>
        {
            return new AmazonS3Client(options.S3AccessKey, options.S3SecretKey, new AmazonS3Config()
            {
                ServiceURL = options.S3Url,
                Timeout = TimeSpan.FromSeconds(5),
                UseHttp = true,
                ForcePathStyle = true,
            });
        });
        var client = collection.BuildServiceProvider().GetService<IAmazonS3>()!;
        var logger = collection.BuildServiceProvider().GetService<ILogger<IAmazonS3>>();
        var buckets = await client.ListBucketsAsync();
        foreach (var seedBucket in options.SeedBuckets)
        {
            if (buckets.Buckets.FirstOrDefault(item => item.BucketName == seedBucket.BucketName) != null) continue;
            
            await client.PutBucketAsync(seedBucket.BucketName);
            logger.LogInformation($"Added bucket {seedBucket.BucketName}");
        }
        collection.AddTransient<IS3StorageService, S3StorageService>();
        return collection;
    } 
}