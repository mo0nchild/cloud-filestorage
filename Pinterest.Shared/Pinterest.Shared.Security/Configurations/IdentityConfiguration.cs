using System.Security.Claims;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Tokens;
using Pinterest.Domain.Authorization.Settings;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Shared.Security.Models;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Shared.Security.Configurations;

public static class IdentityConfiguration
{
    internal static Task<IServiceCollection> AddIdentityServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddAuthentication(UsersAuthenticationOptions.DefaultScheme)
            .AddUsersAuthentication(item => {});
        serviceCollection.AddAuthorization(options =>
        {
            options.AddPolicy(SecurityInfo.Admin, policy => 
                policy.RequireClaim(ClaimTypes.Role, SecurityRole.Admin.ToString()));
            options.AddPolicy(SecurityInfo.Moderator, policy => policy.RequireClaim(ClaimTypes.Role, new []
            {
                SecurityRole.Moderator.ToString(),
                SecurityRole.Admin.ToString()
            }));
            options.AddPolicy(SecurityInfo.User, policy => policy.RequireClaim(ClaimTypes.Role, new []
            {
                SecurityRole.User.ToString()
            }));
        });
        return Task.FromResult(serviceCollection);
    }
    
}