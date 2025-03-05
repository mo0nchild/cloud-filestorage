using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Posts.Infrastructures;
using Pinterest.Application.Posts.Infrastructures.Models;

namespace Pinterest.Documents.Elastic.Configurations;

internal static class PostIndexConfiguration
{
    private static readonly ILogger Logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger(nameof(PostIndexConfiguration));
    public static async Task ConfigureIndexAsync(ElasticsearchClient elasticClient, string indexName)
    {
        if ((await elasticClient.Indices.ExistsAsync(indexName)).Exists)
        {
            Logger.LogInformation($"Index '{indexName}' already exists."); return;
        }
        var response = await elasticClient.Indices.CreateAsync(indexName, configuration =>
        {
            configuration.Settings(AnalyzerConfiguration.BasicSettings);
            configuration.Mappings(mappings =>
            {
                mappings.Properties<PostIndex>(props =>
                {
                    props.Text(item => item.Title, config => config
                        .Analyzer("russian_english_analyzer")
                        .Fields(field => field.Text("autocomplete", cfg => cfg.Analyzer("autocomplete")))
                    );
                    props.Text(item => item.Description, config => config.Analyzer("russian_english_analyzer"));
                    props.Keyword(item => item.Tags);
                    props.Keyword(item => item.PostUuid);
                    props.Completion(item => item.Suggest);
                });
            });
        });
        if (!response.IsSuccess())
        {
            throw new Exception($"Failed to initialize index: '{indexName}' - {response.DebugInformation}");
        }
    }
}