using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Users.Entities;

public class Subscription : BaseEntity
{
    public virtual User Subscriptions { get; set; } = new();
    public virtual User Subscriber { get; set; } = new();
}