namespace CloudStorage.S3Storage.VideoProcessing.Settings;

public class ProcessingSettings
{
    private static readonly string DefaultTempDirectory = "thumbnails";
    
    public required string TempDirectory { get; set; } = DefaultTempDirectory;
}