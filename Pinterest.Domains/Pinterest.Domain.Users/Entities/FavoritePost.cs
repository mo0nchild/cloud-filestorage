using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Users.Entities;

public class FavoritePost : BaseEntity
{
    public Guid PostUuid { get; set; } = Guid.NewGuid();
}