using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Posts.Infrastructures.Models;

namespace Pinterest.Documents.Elastic.Configurations;

internal static class TagIndexConfiguration
{
    private static readonly ILogger Logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger(nameof(TagIndexConfiguration));
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
                mappings.Properties<TagIndex>(props =>
                {
                    props.Text(item => item.Name, config => config
                        .Analyzer("russian_english_analyzer")
                        .Fields(field => field.Text("autocomplete", cfg => cfg.Analyzer("autocomplete")))
                    );
                    props.Keyword(item => item.TagUuid);
                });
            });
        });
        if (!response.IsSuccess())
        {
            throw new Exception($"Failed to initialize index: '{indexName}' - {response.DebugInformation}");
        }
    }
}