namespace Pinterest.Application.Commons.S3Storage;

public interface IS3StorageService
{
    public Task<string?> GetObjectUrlFromStorage(BucketInfo info, DateTime expiry);
    public Task<byte[]?> GetObjectFromStorage(BucketInfo info);

    public Task<bool> LoadObjectToStorage(byte[] file, BucketInfo info);
    public Task<bool> RemoveObjectFromStorage(BucketInfo info);
}