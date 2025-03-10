using AutoMapper;
using CloudStorage.Domain.Core.Models;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Application.Posts.Infrastructures.Models;

public class PostIndex : BaseEntity
{
    public required Guid PostUuid { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = String.Empty;
    public IReadOnlyList<string> Tags { get; set; } = new List<string>();
    public required IReadOnlyList<string> Suggest;
}

public class PostIndexProfile : Profile
{
    public PostIndexProfile()
    {
        CreateMap<PostInfo, PostIndex>()
            .ForMember(dest => dest.PostUuid, opt => opt.MapFrom(src => src.Uuid))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(it => it.Name).ToList()))
            .ForMember(dest => dest.Suggest, opt => opt.MapFrom(src => new List<string>() { src.Title }));
    }
}