﻿// ReSharper disable MemberCanBePrivate.Global
namespace Pinterest.Domain.Authorization.Settings;

public class TokenOptions
{
    public const int AccessExpiresDefault = 1000, RefreshExpiresDefault = 20;
    
    public string AccessSecret { get; set; } = Guid.Empty.ToString();
    /*public string RefreshSecret { get; set; } = Guid.NewGuid().ToString();*/

    public int AccessExpires { get; set; } = AccessExpiresDefault;
    /*public int RefreshExpires { get; set; } = TokenOptions.RefreshExpiresDefault;*/
}