﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Shared.Commons.Configurations;
using Pinterest.Shared.Commons.Middlewares;

namespace Pinterest.Shared.Commons;

public static class Bootstrapper
{
    public static async Task<IServiceCollection> AddCoreConfiguration(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        await serviceCollection.AddModelsMappers();
        await serviceCollection.AddModelsValidators();
        await serviceCollection.AddAppSwagger(configuration);
        await serviceCollection.AddAppCors();
        
        return serviceCollection;
    }
    public static WebApplication UseCoreConfiguration(this WebApplication application)
    {
        application.UseExceptionsHandler();
        application.UseAppSwagger();
        application.UseAppCors();
        
        return application;
    }

}