using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.Posts.Entities;

public class PostInfo : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = default;
    public Guid AuthorUuid { get; set; } = Guid.Empty;
    public EntityStatus Status { get; set; } = default;
    
    public Guid FileUuid { get; set; } = Guid.Empty;
    public Guid? PreviewUuid { get; set; } = default;
    
    public int ViewsCount { get; set; } = default;
    public int LikesCount { get; set; } = default;
    public bool CommentsEnabled { get; set; } = default;
    public bool IsPublic { get; set; } = default;
    
    public virtual List<Guid> GrantedAccess { get; set; } = new();
    public virtual List<TagInfo> Tags { get; set; } = new();
    public virtual List<Comment> Comments { get; set; } = new(); 
}