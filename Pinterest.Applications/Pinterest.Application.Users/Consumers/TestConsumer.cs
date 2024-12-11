using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages;

namespace Pinterest.Application.Users.Consumers;

public class TestConsumer : IMessageConsumer<TestMessage>
{
    public Task ConsumeAsync(TestMessage message)
    {
        Console.WriteLine($"Message: {message.Name}, {message.Uuid}");
        return Task.CompletedTask;
    }
}