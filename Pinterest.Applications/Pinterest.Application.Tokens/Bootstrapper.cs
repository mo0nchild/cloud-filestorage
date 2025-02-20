using System.Collections.ObjectModel;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Tokens.Interfaces;
using Pinterest.Application.Tokens.Services;
using Pinterest.Domain.Authorization.Settings;
using Pinterest.Shared.Contracts;

namespace Pinterest.Application.Tokens;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddTokensServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<ITokenService, TokenService>();
        return Task.FromResult(serviceCollection);
    }
}