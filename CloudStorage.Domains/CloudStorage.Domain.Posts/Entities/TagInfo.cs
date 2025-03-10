using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.Posts.Entities;

public class TagInfo : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public virtual List<PostInfo> Posts { get; set; } = new();
}