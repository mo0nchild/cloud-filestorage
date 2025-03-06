using Pinterest.Api.Posts.Consumers;
using Pinterest.Application.Posts;
using Pinterest.Application.Tokens;
using Pinterest.Database.Posts;
using Pinterest.Documents.Elastic;
using Pinterest.Documents.Mongo;
using Pinterest.Domain.CommonModels.Models;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages.AccountMessages;
using Pinterest.Domain.Messages.UsersMessages;
using Pinterest.MessageBrokers.RabbitMQ;
using Pinterest.MessageBrokers.RabbitMQ.Settings;
using Pinterest.Shared.Commons.Configurations;

namespace Pinterest.Api.Posts.Configurations;

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