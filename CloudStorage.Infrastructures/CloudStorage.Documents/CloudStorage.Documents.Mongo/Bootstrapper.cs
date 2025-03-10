using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CloudStorage.Documents.Mongo.Configurations;
using CloudStorage.Documents.Mongo.Infrastructures;
using CloudStorage.Documents.Mongo.Settings;
using CloudStorage.Domain.Core.Models;
using CloudStorage.Domain.Core.Repositories;

namespace CloudStorage.Documents.Mongo;

public static class Bootstrapper
{
    private static readonly string MongoSection = "Mongo";

    public static Task<IServiceCollection> AddMongoClient(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.Configure<MongoDbSettings>(configuration.GetSection(MongoSection));
        serviceCollection.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(MongoClientSettings.FromConnectionString(settings.ConnectionString));
        });
        return Task.FromResult(serviceCollection);
    }
    public static Task<IServiceCollection> AddMongoDbServices<TDocument>(this IServiceCollection serviceCollection, string collectionName)
        where TDocument : BaseEntity
    {
        MappingConfiguration.ConfigureMappings<TDocument>();
        serviceCollection.AddTransient<IDocumentRepository<TDocument>>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoRepository<TDocument>(client, settings.DatabaseName, collectionName);
        });
        return Task.FromResult(serviceCollection);
    }
}