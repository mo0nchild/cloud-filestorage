using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Users.Entities;

public class UserTheme : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}