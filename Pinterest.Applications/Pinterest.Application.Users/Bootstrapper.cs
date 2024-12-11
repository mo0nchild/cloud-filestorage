using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Users.Consumers;
using Pinterest.Application.Users.Interfaces;
using Pinterest.Application.Users.Services;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages;

namespace Pinterest.Application.Users;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddUsersApplication(this IServiceCollection collection)
    {
        collection.AddTransient<IUserService, UserService>();
        collection.RegistrateProducer<TestConsumer, TestMessage>();
        return Task.FromResult(collection);
    }
}