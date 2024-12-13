using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Authorization.Entities;

public class AccountInfo : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}