using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudStorage.Application.Accounts.Interfaces;
using CloudStorage.Application.Accounts.Services;

namespace CloudStorage.Application.Accounts;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddAccountServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IAccountsService, AccountService>();
        serviceCollection.AddTransient<IRegistrateAccount, RegistrateAccount>();
        return Task.FromResult(serviceCollection);
    }
}