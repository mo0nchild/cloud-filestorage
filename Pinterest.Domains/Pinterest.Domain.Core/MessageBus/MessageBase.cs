namespace Pinterest.Domain.Core.MessageBus;

public abstract class MessageBase
{
    public Guid Uuid = Guid.NewGuid();
    public DateTime Timestamp = DateTime.Now;
}