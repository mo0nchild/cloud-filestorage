using Microsoft.AspNetCore.Authentication;

namespace CloudStorage.Shared.Security.Settings;

public class UsersAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "MyAuthenticationScheme";
}