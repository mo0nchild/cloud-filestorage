namespace CloudStorage.Domain.FileStorage.Settings;

public class StorageSettings
{
    private static readonly int DefaultTtl = 5000, DefaultNotCompletedTtl = 10000;
    private static readonly int DefaultChuckSize = 5 * 1024 * 1024;
    
    public int NotUsingTtl { get; set; } = DefaultTtl;
    public int NotCompletedTtl { get; set; } = DefaultNotCompletedTtl;

    public int MaxChunkSize { get; set; } = DefaultChuckSize;
    public required IReadOnlyList<BucketByExtension> BucketByExtensions { get; set; }
}