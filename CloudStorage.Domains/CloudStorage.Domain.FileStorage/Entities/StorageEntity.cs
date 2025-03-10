using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.FileStorage.Entities;

public class StorageEntity : BaseEntity
{
    public Guid FileUuid { get; set; } = Guid.NewGuid();
    
    public string OriginalName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;

    public long? FileSize { get; set; } = default;
    public string ContentType { get; set; } = string.Empty;
    public ThumbnailFile? Thumbnail { get; set; } = default;
    
    public string UploadId { get; set; } = string.Empty;
    public bool IsUsing { get; set; } = default;
    public bool IsUploaded { get; set; } = default;
}
public class ThumbnailFile
{
    public string FileName { get; set; } = string.Empty;
}