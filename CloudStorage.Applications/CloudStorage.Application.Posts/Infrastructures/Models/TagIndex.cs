using AutoMapper;
using CloudStorage.Domain.Core.Models;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Application.Posts.Infrastructures.Models;

public class TagIndex : BaseEntity
{
    public required Guid TagUuid { get; set; }
    public required string Name { get; set; }
}

public class TagIndexProfile : Profile 
{
    public TagIndexProfile()
    {
        CreateMap<TagInfo, TagIndex>()
            .ForMember(dest => dest.TagUuid, opt => opt.MapFrom(src => src.Uuid))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}
