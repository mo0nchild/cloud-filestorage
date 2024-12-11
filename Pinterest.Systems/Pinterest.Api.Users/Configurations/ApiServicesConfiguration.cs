using Microsoft.VisualBasic;
using Pinterest.Application.Users;
using Pinterest.Database.Users;
using Pinterest.Domain.Messages;
using Pinterest.MessageBrokers.RabbitMQ;
using Pinterest.Shared.Commons.Configurations;

namespace Pinterest.Api.Users.Configurations;

public static class ApiServicesConfiguration
{
    public static async Task<IServiceCollection> AddUsersApiServices(this IServiceCollection collection,
        IConfiguration configuration)
    {
        await collection.AddModelsMappers();
        await collection.AddModelsValidators();
        
        await collection.AddUsersDatabase(configuration);
        await collection.AddUsersApplication();
        await collection.AddConsumerListener<TestMessage>("test", configuration);
        return collection;
    } 
}