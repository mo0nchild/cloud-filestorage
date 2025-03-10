using System.Collections.ObjectModel;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudStorage.Application.Tokens.Interfaces;
using CloudStorage.Application.Tokens.Services;
using CloudStorage.Domain.Authorization.Settings;
using CloudStorage.Shared.Contracts;

namespace CloudStorage.Application.Tokens;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddTokensServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<ITokenService, TokenService>();
        return Task.FromResult(serviceCollection);
    }
}