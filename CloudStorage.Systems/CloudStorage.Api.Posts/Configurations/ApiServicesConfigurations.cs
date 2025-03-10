using CloudStorage.Api.Posts.Consumers;
using CloudStorage.Application.Posts;
using CloudStorage.Application.Tokens;
using CloudStorage.Database.Posts;
using CloudStorage.Documents.Elastic;
using CloudStorage.Documents.Mongo;
using CloudStorage.Domain.CommonModels.Models;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.AccountMessages;
using CloudStorage.Domain.Messages.UsersMessages;
using CloudStorage.MessageBrokers.RabbitMQ;
using CloudStorage.MessageBrokers.RabbitMQ.Settings;
using CloudStorage.Shared.Commons.Configurations;

namespace CloudStorage.Api.Posts.Configurations;

public static class ApiServicesConfigurations
{
    private static readonly string UsersCollectionName = "ValidUsers";
    private static readonly string PostsServiceName = "PostsService";
    public static async Task<IServiceCollection> AddPostsApiServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        await serviceCollection.AddMongoClient(configuration);
        await serviceCollection.AddMongoDbServices<ValidUserInfo>(UsersCollectionName);
        await serviceCollection.AddElasticSearch(configuration);
        
        await serviceCollection.AddPostsDatabase(configuration);
        await serviceCollection.AddPostsServices(configuration);
        await serviceCollection.AddProducerService(configuration);
        
        await MessageConsumerRegistator.Registrate<CreatedUserConsumer, CreatedUserMessage>(serviceCollection);
        await serviceCollection.AddConsumerListener<CreatedUserMessage>(new RoutingOptions()
        {
            QueueName = $"{PostsServiceName}-{CreatedUserMessage.RoutingPath}",
            ExchangeName = CreatedUserMessage.RoutingPath
        }, configuration);
        
        await MessageConsumerRegistator.Registrate<RemovedUserConsumer, RemoveUserMessage>(serviceCollection);
        await serviceCollection.AddConsumerListener<RemoveUserMessage>(new RoutingOptions()
        {
            QueueName = $"{PostsServiceName}-{RemoveUserMessage.RoutingPath}",
            ExchangeName = RemoveUserMessage.RoutingPath
        }, configuration);
        return serviceCollection;
    }
}