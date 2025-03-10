using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.Users.Entities;

public class Subscription : BaseEntity
{
    public virtual User Subscriber { get; set; } = new();
    public virtual User Author { get; set; } = new();
}