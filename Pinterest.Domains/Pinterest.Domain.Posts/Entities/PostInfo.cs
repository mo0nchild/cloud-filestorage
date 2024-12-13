using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Posts.Entities;

public class PostInfo : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public Guid UserUuid { get; set; } = Guid.Empty;
}