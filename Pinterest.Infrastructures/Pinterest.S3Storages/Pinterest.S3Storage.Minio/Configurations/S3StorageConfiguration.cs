using Microsoft.Extensions.Configuration;
using Pinterest.S3Storage.Minio.Settings;

namespace Pinterest.S3Storage.Minio.Configurations;

public static class S3StorageConfiguration
{
    private static readonly string S3StorageSection = "S3Storage";
    
    public static S3StorageOptions GetS3StorageOptions(IConfiguration configuration)
    {
        var options = configuration.GetSection(S3StorageSection).Get<S3StorageOptions>();
        if (options == null) throw new Exception($"Not found {S3StorageSection} options in configuration");
        return options;
    }
}