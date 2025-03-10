// ReSharper disable CollectionNeverUpdated.Global
namespace CloudStorage.S3Storage.Minio.Settings;

public class S3StorageOptions
{
    public string S3AccessKey { get; set; } = string.Empty;
    public string S3SecretKey { get; set; } = string.Empty;

    public string S3Url { get; set; } = string.Empty;
}