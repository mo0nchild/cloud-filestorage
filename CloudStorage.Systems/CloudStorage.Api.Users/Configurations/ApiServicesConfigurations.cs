using CloudStorage.Api.Users.Consumers;
using CloudStorage.Application.Tokens;
using CloudStorage.Application.Users;
using CloudStorage.Application.Users.Services;
using CloudStorage.Database.Users;
using CloudStorage.Documents.Mongo;
using CloudStorage.Domain.CommonModels.Models;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.AccountMessages;
using CloudStorage.Domain.Messages.SagaMessages.CreateAccountSaga;
using CloudStorage.Domain.Users.Entities;
using CloudStorage.GrpcServices.Users;
using CloudStorage.MessageBrokers.RabbitMQ;
using CloudStorage.MessageBrokers.RabbitMQ.Settings;
using CloudStorage.MessageBrokers.Saga;
using CloudStorage.MessageBrokers.Saga.Settings;
using CloudStorage.Shared.Commons.Configurations;
using CloudStorage.Shared.Security.Configurations;

namespace CloudStorage.Api.Users.Configurations;

public static class ApiServicesConfigurations
{
    private static readonly string RegistrateAccountSagaName = "RegistrateAccount";
    private static readonly string PostsCollectionName = "ValidPosts";
    public static async Task<IServiceCollection> AddUsersApiServices(this IServiceCollection serviceCollection,
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