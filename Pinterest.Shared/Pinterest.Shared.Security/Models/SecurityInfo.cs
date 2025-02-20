namespace Pinterest.Shared.Security.Models;

public static class SecurityInfo
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Moderator = "Moderator";
}

public enum SecurityRole
{
    User,
    Moderator,
    Admin,
}