using Microsoft.Extensions.DependencyInjection;

namespace Pinterest.Domain.Core.MessageBus;

public interface IMessageConsumer<in TMessage> where TMessage : MessageBase
{
    public Task ConsumeAsync(TMessage message);
}

public static class MessageConsumerRegistator
{
    public static Task RegistrateProducer<TConsumer, TMessage>(this IServiceCollection collection)
        where TMessage : MessageBase
        where TConsumer : class, IMessageConsumer<TMessage>
    {
        collection.AddSingleton<IMessageConsumer<TMessage>, TConsumer>();
        return Task.FromResult(collection);
    }
}