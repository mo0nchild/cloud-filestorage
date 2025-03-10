using CloudStorage.Api.FileStorage.Consumers;
using CloudStorage.Application.FileStorage;
using CloudStorage.Application.Tokens;
using CloudStorage.Documents.Mongo;
using CloudStorage.Documents.Mongo;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.FileStorage.Entities;
using CloudStorage.Domain.FileStorage.Settings;
using CloudStorage.Domain.Messages.FileStorageMessages;
using CloudStorage.MessageBrokers.RabbitMQ;
using CloudStorage.MessageBrokers.RabbitMQ.Settings;
using CloudStorage.S3Storage.Minio;
using CloudStorage.S3Storage.VideoProcessing;
using CloudStorage.Shared.Commons.Configurations;

namespace CloudStorage.Api.FileStorage.Configurations;

public static class ApiServicesConfigurations
{
    private static readonly string StorageCollectionName = "StorageEntities";
    public static async Task<IServiceCollection> AddApiServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        await serviceCollection.AddS3StorageService(configuration);
        await serviceCollection.AddFileStorageServices(configuration);
        await serviceCollection.AddVideoProcessingServices(configuration);
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