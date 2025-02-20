using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Users.Interfaces;
using Pinterest.Application.Users.Services;
using Pinterest.Application.Users.Services.Validators;

namespace Pinterest.Application.Users;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddUsersServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IUserService, UserService>();
        serviceCollection.AddTransient<IUserValidators, UserValidators>();
        return Task.FromResult(serviceCollection);
    }
}