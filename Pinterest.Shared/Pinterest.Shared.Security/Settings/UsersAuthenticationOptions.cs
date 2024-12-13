using Microsoft.AspNetCore.Authentication;

namespace Pinterest.Shared.Security.Settings;

public class UsersAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "MyAuthenticationScheme";
}