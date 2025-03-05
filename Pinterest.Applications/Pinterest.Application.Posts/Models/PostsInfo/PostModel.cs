using AutoMapper;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Models.PostsInfo;

public class PostModel
{
    public required Guid PostUuid { get; set; } = Guid.NewGuid();
    public required DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    
    public required string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public required Guid AuthorUuid { get; set; } = Guid.Empty;
    
    public required Guid FileUuid { get; set; } = Guid.Empty;
    public Guid? PreviewUuid { get; set; }
    
    public required int ViewsCount { get; set; }
    public required int LikesCount { get; set; }
    
    public required bool CommentsEnabled { get; set; }
    public required bool IsPublic { get; set; }
    public IReadOnlyList<TagModel> Tags { get; set; } = new List<TagModel>();
}
public class TagModel
{
    public required string Name { get; set; }
    public required int PostsCount { get; set; }
}
public class PostModelProfile : Profile
{
    public PostModelProfile()
    {
        CreateMap<TagInfo, TagModel>()
            .ForMember(dest => dest.PostsCount, opt => opt.MapFrom(src => src.Posts.Count))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        CreateMap<PostInfo, PostModel>()
            .ForMember(dest => dest.PostUuid, opt => opt.MapFrom(src => src.Uuid))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
    }
}