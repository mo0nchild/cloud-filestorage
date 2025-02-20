﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.GrpcServices.Core.Settings;

namespace Pinterest.GrpcServices.Core.Configurations;

public static class GrpcConfiguration
{
    private static readonly string GrpcSection = "GrpcServices";
    
    public static Task<IServiceCollection> AddGrpcServiceSetting<TOptions>(this IServiceCollection serviceCollection,
        IConfiguration configuration) 
        where TOptions : GrpcSettingsBase, new()
    {
        var servicesConfig = configuration.GetSection(GrpcSection);
        return Task.FromResult(serviceCollection.Configure<TOptions>(servicesConfig.GetSection(typeof(TOptions).Name)));
    }
}