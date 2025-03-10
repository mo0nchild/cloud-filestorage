using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudStorage.Application.Tokens;
using CloudStorage.Domain.Authorization.Settings;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Shared.Security.Models;
using CloudStorage.Shared.Security.Settings;

namespace CloudStorage.Shared.Security.Configurations;

public static class IdentityConfiguration
{
    private static AuthorizationPolicyBuilder RequireRestUserChecking(this AuthorizationPolicyBuilder builder)
    {
        return builder.RequireAssertion(context =>
        {
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null) return false;

            var isGrpc = httpContext.Request.ContentType?.StartsWith("application/grpc") == true;
            if (isGrpc) return true;
            
            return httpContext.User.Identity?.IsAuthenticated == true 
                   && (httpContext.User.IsInRole(SecurityRole.User.ToString()) 
                   || httpContext.User.IsInRole(SecurityRole.Moderator.ToString())
                   || httpContext.User.IsInRole(SecurityRole.Admin.ToString()));
        });
    }
    internal static Task<IServiceCollection> AddIdentityServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddAuthentication(UsersAuthenticationOptions.DefaultScheme)
            .AddUsersAuthentication(item => {});
        serviceCollection.AddAuthorization(options =>
        {
            options.AddPolicy(SecurityInfo.Admin, policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, SecurityRole.Admin.ToString());
                policy.RequireAuthenticatedUser();
            });
            options.AddPolicy(SecurityInfo.Moderator, policy => policy.RequireClaim(ClaimTypes.Role, new []
            {
                SecurityRole.Moderator.ToString(),
                SecurityRole.Admin.ToString()
            }).RequireAuthenticatedUser());
            options.AddPolicy(SecurityInfo.User, policy => policy.RequireRestUserChecking());
        });
        return Task.FromResult(serviceCollection);
    }
    
}