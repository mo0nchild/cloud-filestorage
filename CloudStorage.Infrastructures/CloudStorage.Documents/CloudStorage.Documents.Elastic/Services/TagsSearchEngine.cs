using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Commons.Models;
using CloudStorage.Application.Posts.Infrastructures;
using CloudStorage.Application.Posts.Infrastructures.Interfaces;
using CloudStorage.Application.Posts.Infrastructures.Models;
using CloudStorage.Documents.Elastic.Settings;
using SearchRequest = CloudStorage.Application.Posts.Infrastructures.Interfaces.SearchRequest;

namespace CloudStorage.Documents.Elastic.Services;

internal class TagsSearchEngine : ISearchEngine<TagIndex>
{
    private readonly string _indexName = nameof(TagIndex).ToLower();
    private readonly ElasticsearchClient _elasticClient;

    public TagsSearchEngine(ElasticsearchClient elasticClient, IOptions<ElasticSettings> options,
        ILogger<TagsSearchEngine> logger)
    {
        Settings = options.Value;
        Logger = logger;
        _elasticClient = elasticClient;
    }
    private ILogger<TagsSearchEngine> Logger { get; }
    private ElasticSettings Settings { get; }
    public async Task IndexPostAsync(TagIndex postIndex)
    {
        var indexRequest = new IndexRequest<TagIndex>(postIndex, _indexName, Guid.NewGuid());
        var indexResponse = await _elasticClient.IndexAsync(indexRequest);
        if (!indexResponse.IsSuccess())
        {
            Logger.LogWarning($"Failed to index: {_indexName} - {indexResponse.DebugInformation}");
            throw new ProcessException($"Error with adding post to search index: {_indexName}");
        }
    }
    public async Task RemovePostAsync(Guid postUuid)
    {
        var searchResponse = await _elasticClient.SearchAsync<TagIndex>(search => search
            .Index(_indexName)
            .Query(query => query.Term(term => term.Field(f => f.TagUuid).Value(postUuid.ToString())))
        );
        if (searchResponse.IsValidResponse && searchResponse.Documents.Any())
        {
            var documentId = searchResponse.Hits.First().Id;
            var deleteResponse = await _elasticClient.DeleteAsync(new DeleteRequest(_indexName, documentId!));

            if (!deleteResponse.IsValidResponse)
            {
                Logger.LogWarning($"Failed to delete from index: {_indexName}' - {searchResponse.DebugInformation}");
                throw new ProcessException($"Failed to delete from index: {_indexName}");
            }
        }
        else {
            Logger.LogWarning($"Cannot found post '{postUuid}' in search index: {_indexName} " +
                              $"- {searchResponse.DebugInformation}");
            throw new ProcessException($"Cannot found post '{postUuid}' in search index: {_indexName}");
        }
    }
    public async Task UpdatePostAsync(TagIndex postIndex)
    {
        var searchResponse = await _elasticClient.SearchAsync<TagIndex>(search => search
            .Index(_indexName)
            .Query(query => query.Term(term => term.Field(f => f.TagUuid).Value(postIndex.TagUuid.ToString())))
        );
        if (!searchResponse.IsValidResponse || !searchResponse.Documents.Any())
        {
            Logger.LogWarning($"Cannot find tag '{postIndex.TagUuid}' in search index: {_indexName} " +
                              $"- {searchResponse.DebugInformation}");
            throw new ProcessException($"Cannot find tag '{postIndex.TagUuid}' in search index: {_indexName}");
        }
        var documentId = searchResponse.Hits.First().Id;
        var updateResponse = await _elasticClient.UpdateAsync<TagIndex, object>(_indexName, documentId!,
            configureRequest: update => update.Doc(new { Title = postIndex.Name })
        );
        if (!updateResponse.IsValidResponse)
        {
            Logger.LogWarning($"Failed to update tag '{postIndex.TagUuid}' in index: {_indexName} " +
                              $"- {updateResponse.DebugInformation}");
            throw new ProcessException($"Failed to update tag '{postIndex.TagUuid}' in index: {_indexName}");
        }
    }
    public async Task<PagedResult<Guid>> SearchPostsAsync(SearchRequest searchRequest)
    {
        var searchResponse = await _elasticClient.SearchAsync<TagIndex>(descriptor =>
        {
            descriptor.From(searchRequest.QueryRange.From);
            descriptor.Query(query => query.Match(match => match.Field(select => select.Name)
                .Query(searchRequest.QueryValue)
                .Fuzziness(new Fuzziness("AUTO"))
            ));
            descriptor.Size(searchRequest.QueryRange.ListSize);
        });
        if (!searchResponse.IsValidResponse)
        {
            Logger.LogWarning($"Failed search in '{_indexName}': {searchRequest.QueryValue} " +
                              $"- {searchResponse.DebugInformation}");
            throw new ProcessException($"Error searching in index: {_indexName}, query: {searchRequest.QueryValue}");
        }
        return new PagedResult<Guid>()
        {
            Items = searchResponse.Documents.Select(item => item.TagUuid).ToList(),
            TotalCount = searchResponse.Total,
        };
    }
}