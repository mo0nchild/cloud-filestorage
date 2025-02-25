using Pinterest.Api.Users.Consumers;
using Pinterest.Application.Tokens;
using Pinterest.Application.Users;
using Pinterest.Application.Users.Services;
using Pinterest.Database.Users;
using Pinterest.Documents.Mongo;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages.AccountMessages;
using Pinterest.Domain.Messages.SagaMessages.CreateAccountSaga;
using Pinterest.Domain.Users.Entities;
using Pinterest.GrpcServices.Users;
using Pinterest.MessageBrokers.RabbitMQ;
using Pinterest.MessageBrokers.RabbitMQ.Settings;
using Pinterest.MessageBrokers.Saga;
using Pinterest.MessageBrokers.Saga.Settings;
using Pinterest.Shared.Commons.Configurations;
using Pinterest.Shared.Security.Configurations;

namespace Pinterest.Api.Users.Configurations;

public static class ApiServicesConfigurations
{
    private static readonly string RegistrateAccountSagaName = "RegistrateAccount";
    private static readonly string PostsCollectionName = "ValidPosts";
    public static async Task<IServiceCollection> AddApiServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        await serviceCollection.AddMongoClient(configuration);
        await serviceCollection.AddMongoDbServices<ValidPostInfo>(PostsCollectionName);
        await serviceCollection.AddUsersDatabase(configuration);
        await serviceCollection.AddUsersGrpcServices(configuration);
        await serviceCollection.AddUsersServices();
        
        await serviceCollection.AddProducerService(configuration);
        await MessageConsumerRegistator.Registrate<RemoveAccountConsumer, RemoveAccountMessage>(serviceCollection);
        await serviceCollection.AddConsumerListener<RemoveAccountMessage>(new RoutingOptions()
        {
            QueueName = $"UserService-{RemoveAccountMessage.RoutingPath}",
            ExchangeName = RemoveAccountMessage.RoutingPath
        }, configuration);
        await serviceCollection.AddSagaService<UserSagaService, CreateUserPayload, CreateUserCompensation>(
            new SagaServiceSetting()
            {
                SagaName = RegistrateAccountSagaName,
                ServiceName = CreateUserServiceInfo.ServiceName,
            },
            configuration);
        return serviceCollection;
    }
}