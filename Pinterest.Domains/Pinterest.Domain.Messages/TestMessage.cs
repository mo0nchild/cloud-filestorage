using Pinterest.Domain.Core.MessageBus;

namespace Pinterest.Domain.Messages;

public class TestMessage : MessageBase
{
    public string Name { get; set; }
}