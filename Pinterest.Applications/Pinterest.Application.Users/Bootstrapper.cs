using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Users.Interfaces;
using Pinterest.Application.Users.Services;

namespace Pinterest.Application.Users;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddUsersServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IUserValidators, UserValidators>();
        serviceCollection.AddTransient<IUserService, UserService>();
        
        serviceCollection.AddTransient<IFavoritesPostService, FavoritesPostService>();
        serviceCollection.AddTransient<ISubscribersService, SubscribersService>();
        return Task.FromResult(serviceCollection);
    }
}