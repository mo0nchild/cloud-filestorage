using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.Users.Entities;

public class FavoritePost : BaseEntity
{
    public Guid PostUuid { get; set; } = Guid.NewGuid();
    public virtual User User { get; set; } = new();
}