using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.Authorization.Entities;

public class AccountInfo : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public Guid UserUuid { get; set; } = Guid.NewGuid();
    public AccountRole Role { get; set; } = default;
    public string? RefreshToken { get; set; }
}
public enum AccountRole
{
    User,
    Moderator,
    Admin,
}