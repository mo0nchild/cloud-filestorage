namespace CloudStorage.Application.FileStorage.Infrastructures.Interfaces;

public interface IBucketManager
{
    Task CreateBucketAsync(string bucketName);
    Task DeleteBucketAsync(string bucketName);
    Task<int> GetFilesInBucketCount(string bucketName);
    Task<IReadOnlyList<string>> GetBucketsListAsync();
}