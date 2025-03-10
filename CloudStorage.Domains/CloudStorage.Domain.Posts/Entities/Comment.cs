using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.Posts.Entities;

public class Comment : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public Guid UserUuid { get; set; } = Guid.Empty;
    
    public int LikesCount { get; set; } = default;
    
    public virtual PostInfo Post { get; set; } = new();
    public virtual Comment? ParentComment { get; set; } = default;
    
    public virtual List<Comment> SubComments { get; set; } = new(); 
}