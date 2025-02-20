using Pinterest.Api.FileStorage.Consumers;
using Pinterest.Application.FileStorage;
using Pinterest.Application.Tokens;
using Pinterest.Documents.Mongo;
using Pinterest.Documents.Mongo;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.FileStorage.Entities;
using Pinterest.Domain.FileStorage.Settings;
using Pinterest.Domain.Messages.FileStorageMessages;
using Pinterest.MessageBrokers.RabbitMQ;
using Pinterest.MessageBrokers.RabbitMQ.Settings;
using Pinterest.S3Storage.Minio;
using Pinterest.Shared.Commons.Configurations;

namespace Pinterest.Api.FileStorage.Configurations;

public static class ApiServicesConfigurations
{
    private static readonly string StorageCollectionName = "StorageEntities";
    public static async Task<IServiceCollection> AddApiServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        await serviceCollection.AddS3StorageService(configuration);
        await serviceCollection.AddFileStorageServices(configuration);
        
        await serviceCollection.AddProducerService(configuration);
        
        await MessageConsumerRegistator.Registrate<DeleteFileConsumer, FileRemovedMessage>(serviceCollection);
        await serviceCollection.AddConsumerListener<FileRemovedMessage>(new RoutingOptions()
        {
            QueueName = FileRemovedMessage.RoutingPath,
        }, configuration);
        
        await serviceCollection.AddMongoClient(configuration);
        await serviceCollection.AddMongoDbServices<StorageEntity>(StorageCollectionName);
        return serviceCollection;
    }
}