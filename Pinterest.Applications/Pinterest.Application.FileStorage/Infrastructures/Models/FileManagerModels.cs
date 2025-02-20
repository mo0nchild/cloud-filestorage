using AutoMapper;
using Pinterest.Application.FileStorage.Models;
using Pinterest.Domain.FileStorage.Entities;

namespace Pinterest.Application.FileStorage.Infrastructures.Models;

public class StoringFileInfo
{
    public required string Bucket { get; set; }
    public required string FileName { get; set; }
}

public class StoringFileInfoProfile : Profile
{
    public StoringFileInfoProfile()
    {
        CreateMap<StorageEntity, StoringFileInfo>()
            .ForMember(prop => prop.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(prop => prop.Bucket, opt => opt.MapFrom(src => src.BucketName));
    }
}

public class UploadData
{
    public required StoringFileInfo FileInfo { get; set; }
    
    public required int PartNumber { get; set; }
    public required string UploadId { get; set; }
    
    public required Stream ChuckData { get; set; }
}
public record UploadDataResult(string ETag);

public class CompleteUploadData
{
    public required StoringFileInfo FileInfo { get; set; }
    
    public required string UploadId { get; set; }
    public required IReadOnlyList<PartInfo> Parts { get; set; }
}
public record FileMetadata(long FileSize, string ContentType);