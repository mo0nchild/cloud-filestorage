using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Users.Entities;

public class ValidPostInfo : BaseEntity
{
    public Guid PostUuid { get; set; } = Guid.Empty;
}