using AutoMapper;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Models.PostsInfo;

public class PostModel
{
    public virtual Guid Uuid { get; set; } = Guid.NewGuid();
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = default;
    public Guid AuthorUuid { get; set; } = Guid.Empty;
    
    public Guid FileUuid { get; set; } = Guid.Empty;
    public Guid? PreviewUuid { get; set; } = default;
    
    public int ViewsCount { get; set; } = default;
    public int LikesCount { get; set; } = default;
    public bool CommentsEnabled { get; set; } = default;
    public bool IsPublic { get; set; } = default;
    
    public virtual List<string> Tags { get; set; } = new();
    public virtual List<Comment> Comments { get; set; } = new(); 
}

public class PostModelProfile : Profile
{
    public PostModelProfile() => CreateMap<PostInfo, PostModel>();
}