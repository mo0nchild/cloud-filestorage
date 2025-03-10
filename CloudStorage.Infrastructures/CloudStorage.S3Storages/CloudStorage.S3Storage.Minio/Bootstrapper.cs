using System.Net;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.FileStorage.Infrastructures;
using CloudStorage.Application.FileStorage.Infrastructures.Interfaces;
using CloudStorage.S3Storage.Minio.Configurations;
using CloudStorage.S3Storage.Minio.Infrastructures;
using CloudStorage.S3Storage.Minio.Settings;
// ReSharper disable ConvertToConstant.Local
// ReSharper disable ConvertToLambdaExpression

namespace CloudStorage.S3Storage.Minio;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddS3StorageService(this IServiceCollection collection,
        IConfiguration configuration)
    {
        var s3StorageSettings = S3StorageConfiguration.GetS3StorageOptions(configuration);
        collection.AddSingleton<IAmazonS3, AmazonS3Client>(provider =>
        {
            return new AmazonS3Client(s3StorageSettings.S3AccessKey, s3StorageSettings.S3SecretKey, new AmazonS3Config()
            {
                ServiceURL = s3StorageSettings.S3Url,
                Timeout = TimeSpan.FromSeconds(5),
                UseHttp = true,
                ForcePathStyle = true,
            });
        });
        collection.AddTransient<IFileManager, FileManager>();
        collection.AddTransient<IBucketManager, BucketManager>();
        return Task.FromResult(collection);
    } 
}