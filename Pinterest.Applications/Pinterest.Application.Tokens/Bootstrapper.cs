using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Tokens.Interfaces;
using Pinterest.Application.Tokens.Services;
using Pinterest.Domain.Authorization.Settings;

namespace Pinterest.Application.Tokens;

public static class Bootstrapper
{
    private static readonly string TokenSection = "Tokens";
    public static Task<IServiceCollection> AddTokensServices(this IServiceCollection collection,
        IConfiguration configuration)
    {
        collection.Configure<TokenOptions>(configuration.GetSection(TokenSection));
        collection.AddTransient<ITokenService, TokenService>();
        return Task.FromResult(collection);
    }
}