using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.Users.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public EntityStatus Status { get; set; } = default;
    public string? PhotoPath { get; set; } = default;
    
    public virtual List<string> UserThemes { get; set; } = new();
    public virtual List<FavoritePost> FavoritesPosts { get; set; } = new();
    
    public virtual List<Subscription> Subscriptions { get; set; } = new();
    public virtual List<Subscription> Subscribers { get; set; } = new();
}