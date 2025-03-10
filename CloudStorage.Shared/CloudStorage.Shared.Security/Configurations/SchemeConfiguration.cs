using Microsoft.AspNetCore.Authentication;
using CloudStorage.Shared.Security.Infrastructure;
using CloudStorage.Shared.Security.Settings;

namespace CloudStorage.Shared.Security.Configurations;

public static class SchemeConfiguration
{
    internal static AuthenticationBuilder AddUsersAuthentication(this AuthenticationBuilder builder, 
        Action<UsersAuthenticationOptions> configuration)
    {
        return builder.AddScheme<UsersAuthenticationOptions, UsersAuthenticationScheme>(
            UsersAuthenticationOptions.DefaultScheme, configuration);
    }
}