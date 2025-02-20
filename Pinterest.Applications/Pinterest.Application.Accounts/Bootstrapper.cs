using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Accounts.Interfaces;
using Pinterest.Application.Accounts.Services;

namespace Pinterest.Application.Accounts;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddAccountServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IAccountsService, AccountService>();
        serviceCollection.AddTransient<IRegistrateAccount, RegistrateAccount>();
        return Task.FromResult(serviceCollection);
    }
}