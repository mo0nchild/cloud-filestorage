using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.MessageBrokers.RabbitMQ.Implementations;
using Pinterest.MessageBrokers.RabbitMQ.Infrastuctures;
using Pinterest.MessageBrokers.RabbitMQ.Settings;

namespace Pinterest.MessageBrokers.RabbitMQ;

public static class Bootstrapper
{
    private static readonly string BrokerSection = "MessageBroker";

    public static Task<IServiceCollection> AddProducerService(this IServiceCollection collection,
        string queueName, IConfiguration configuration)
    {
        collection.Configure<BrokerBaseSetting>(configuration.GetSection(BrokerSection));
        collection.AddTransient<IMessageProducer, MessageProducer>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<BrokerBaseSetting>>();
            var logger = provider.GetRequiredService<ILogger<MessageProducer>>();
            var messageProducer = new MessageProducer(options, logger);

            messageProducer.InitializeAsync().Wait();
            return messageProducer;
        });
        return Task.FromResult(collection);
    }

    public static Task<IServiceCollection> AddConsumerListener<TMessage>(this IServiceCollection collection,
        string queueName, IConfiguration configuration) where TMessage : MessageBase
    {
        collection.Configure<BrokerBaseSetting>(configuration.GetSection(BrokerSection));
        collection.AddHostedService<ConsumerListener<TMessage>>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<BrokerBaseSetting>>();
            var consumer = provider.GetRequiredService<IMessageConsumer<TMessage>>();
            var logger = provider.GetRequiredService<ILogger<ConsumerListener<TMessage>>>();
            
            var consumerListener = new ConsumerListener<TMessage>(queueName, consumer, options, logger);
            consumerListener.InitializeAsync().Wait();
            return consumerListener;
        });
        return Task.FromResult(collection);
    }
}