using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Users.Entities;

public class Subscription : BaseEntity
{
    public virtual User Subscriber { get; set; } = new();
    public virtual User Author { get; set; } = new();
}