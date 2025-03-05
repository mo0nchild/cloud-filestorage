using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Posts.Entities;

public class TagInfo : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public virtual List<PostInfo> Posts { get; set; } = new();
}