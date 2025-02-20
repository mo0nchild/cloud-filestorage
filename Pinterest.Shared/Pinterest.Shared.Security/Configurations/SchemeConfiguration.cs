using Microsoft.AspNetCore.Authentication;
using Pinterest.Shared.Security.Infrastructure;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Shared.Security.Configurations;

public static class SchemeConfiguration
{
    internal static AuthenticationBuilder AddUsersAuthentication(this AuthenticationBuilder builder, 
        Action<UsersAuthenticationOptions> configuration)
    {
        return builder.AddScheme<UsersAuthenticationOptions, UsersAuthenticationScheme>(
            UsersAuthenticationOptions.DefaultScheme, configuration);
    }
}