namespace CloudStorage.Domain.FileStorage.Settings;

public class BucketByExtension
{
    public required string BucketName { get; set; }
    
    public IReadOnlyList<string> Extensions { get; set; } = new List<string>();
}