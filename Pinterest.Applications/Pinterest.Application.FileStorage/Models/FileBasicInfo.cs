using AutoMapper;
using Pinterest.Domain.FileStorage.Entities;

namespace Pinterest.Application.FileStorage.Models;

public class FileBasicInfo
{
    public required string ContentType { get; set; }
    public required long FileSize { get; set; }
    public required ThumbnailFile? ThumbnailFile { get; set; }
}

public class FileBasicInfoProfile : Profile
{
    public FileBasicInfoProfile()
    {
        CreateMap<StorageEntity, FileBasicInfo>()
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize))
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
            .ForMember(dest => dest.ThumbnailFile, opt => opt.MapFrom(src => src.Thumbnail));
    }
}