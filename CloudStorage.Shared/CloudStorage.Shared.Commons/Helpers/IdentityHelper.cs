using System.Security.Claims;

namespace CloudStorage.Shared.Commons.Helpers;

public static class IdentityHelper
{
    public static Guid? GetUserUuid(this ClaimsPrincipal principal)
    {
        var uuid = principal.FindFirstValue(ClaimTypes.PrimarySid);
        return Guid.TryParse(uuid, out var result) ? result : null;
    }
    public static string? GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Email);
    }
}