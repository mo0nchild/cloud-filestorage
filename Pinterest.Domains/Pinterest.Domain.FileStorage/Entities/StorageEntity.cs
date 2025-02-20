using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.FileStorage.Entities;

public class StorageEntity : BaseEntity
{
    public Guid FileUuid { get; set; } = Guid.NewGuid();
    
    public string OriginalName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;

    public long? FileSize { get; set; } = default;
    
    public string UploadId { get; set; } = string.Empty;
    public bool IsUsing { get; set; } = default;
    public bool IsUploaded { get; set; } = default;
}