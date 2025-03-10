using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CloudStorage.Application.Posts.Infrastructures;
using CloudStorage.Application.Posts.Infrastructures.Interfaces;
using CloudStorage.Application.Posts.Infrastructures.Models;
using CloudStorage.Documents.Elastic.Configurations;
using CloudStorage.Documents.Elastic.Services;
using CloudStorage.Documents.Elastic.Settings;

namespace CloudStorage.Documents.Elastic;

public static class Bootstrapper
{
    private static readonly string ElasticSection = "Elastic";
    public static async Task<IServiceCollection> AddElasticSearch(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        serviceCollection.Configure<ElasticSettings>(configuration.GetSection(ElasticSection));
        serviceCollection.AddSingleton<ElasticsearchClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<ElasticSettings>>().Value;
            var clientSettings = new ElasticsearchClientSettings(new Uri(settings.ConnectionString))
                .DefaultIndex(settings.DefaultIndex)
                .Authentication(new BasicAuthentication(settings.Username, settings.Password));

            return new ElasticsearchClient(clientSettings);
        });
        await using var provider = serviceCollection.BuildServiceProvider();
        var elasticClient = provider.GetRequiredService<ElasticsearchClient>();

        if (!(await elasticClient.PingAsync()).IsValidResponse)
        {
            throw new Exception("Could not establish connection to Elastic");
        }
        await PostIndexConfiguration.ConfigureIndexAsync(elasticClient, nameof(PostIndex).ToLower());
        await TagIndexConfiguration.ConfigureIndexAsync(elasticClient, nameof(TagIndex).ToLower());

        serviceCollection.AddTransient<ISearchEngine<PostIndex>, PostsSearchEngine>();
        serviceCollection.AddTransient<ISearchEngine<TagIndex>, TagsSearchEngine>();
        return serviceCollection;
    }
}