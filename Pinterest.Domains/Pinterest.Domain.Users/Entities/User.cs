using Pinterest.Domain.Core.Models;
using Pinterest.Domain.Users.Enums;

namespace Pinterest.Domain.Users.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public Gender Gender { get; set; } = default;
    public EntityStatus Status { get; set; } = default;
    public string? PhotoPath { get; set; } = default;
    
    public virtual List<UserTheme> UserThemes { get; set; } = new();
    public virtual List<FavoritePost> FavoritesPosts { get; set; } = new();
    
    public virtual List<Subscription> Subscriptions { get; set; } = new();
    public virtual List<Subscription> Subscribers { get; set; } = new();
}