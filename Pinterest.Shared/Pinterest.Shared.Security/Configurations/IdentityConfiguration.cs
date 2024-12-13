using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Shared.Security.Configurations;

public static class IdentityConfiguration
{
    public static Task<IServiceCollection> AddIdentityServices(this IServiceCollection collection,
        IConfiguration configuration)
    {
        collection.AddAuthentication(UsersAuthenticationOptions.DefaultScheme)
            .AddUsersAuthentication(item => {});
        /*collection.AddAuthorization(options =>
        {

        });*/
        return Task.FromResult(collection);
    }
}