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
using CloudStorage.Domain.Core.Repositories;
using SearchRequest = CloudStorage.Application.Posts.Infrastructures.Interfaces.SearchRequest;

namespace CloudStorage.Documents.Elastic.Services;

internal class PostsSearchEngine : ISearchEngine<PostIndex>
{
    private readonly string _indexName = nameof(PostIndex).ToLower();
    private readonly ElasticsearchClient _elasticClient;

    public PostsSearchEngine(ElasticsearchClient elasticClient, IOptions<ElasticSettings> options,
        ILogger<PostsSearchEngine> logger)
    {
        Settings = options.Value;
        Logger = logger;
        _elasticClient = elasticClient;
    }
    private ILogger<PostsSearchEngine> Logger { get; }
    private ElasticSettings Settings { get; }
    public async Task IndexPostAsync(PostIndex postIndex)
    {
        var indexRequest = new IndexRequest<PostIndex>(postIndex, _indexName, Guid.NewGuid());
        var indexResponse = await _elasticClient.IndexAsync(indexRequest);
        if (!indexResponse.IsSuccess())
        {
            Logger.LogWarning($"Failed to index: {_indexName} - {indexResponse.DebugInformation}");
            throw new ProcessException($"Error with adding post to search index: {_indexName}");
        }
    }
    public async Task RemovePostAsync(Guid postUuid)
    {
        var searchResponse = await _elasticClient.SearchAsync<PostIndex>(search => search
            .Index(_indexName)
            .Query(query => query.Term(term => term.Field(f => f.PostUuid).Value(postUuid.ToString())))
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
    public async Task UpdatePostAsync(PostIndex postIndex)
    {
        var searchResponse = await _elasticClient.SearchAsync<PostIndex>(search => search
            .Index(_indexName)
            .Query(query => query.Term(term => term.Field(f => f.PostUuid).Value(postIndex.PostUuid.ToString())))
        );
        if (!searchResponse.IsValidResponse || !searchResponse.Documents.Any())
        {
            Logger.LogWarning($"Cannot find post '{postIndex.PostUuid}' in search index: {_indexName} " +
                              $"- {searchResponse.DebugInformation}");
            throw new ProcessException($"Cannot find post '{postIndex.PostUuid}' in search index: {_indexName}");
        }
        var documentId = searchResponse.Hits.First().Id;
        var updateResponse = await _elasticClient.UpdateAsync<PostIndex, object>(_indexName, documentId!,
            configureRequest: update => update.Doc(new
            {
                Title = postIndex.Title,
                Description = postIndex.Description,
                Tags = postIndex.Tags
            })
        );
        if (!updateResponse.IsValidResponse)
        {
            Logger.LogWarning($"Failed to update post '{postIndex.PostUuid}' in index: {_indexName} " +
                              $"- {updateResponse.DebugInformation}");
            throw new ProcessException($"Failed to update post '{postIndex.PostUuid}' in index: {_indexName}");
        }
    }
    public async Task<PagedResult<Guid>> SearchPostsAsync(SearchRequest searchRequest)
    {
        var targetFields = new[] { nameof(PostIndex.Title).ToLower(), nameof(PostIndex.Description).ToLower() };
        var searchResponse = await _elasticClient.SearchAsync<PostIndex>(descriptor =>
        {
            descriptor.From(searchRequest.QueryRange.From);
            descriptor.Query(query => query.Bool(configure =>
                {
                    configure.Must(mustDescriptor => mustDescriptor
                        .MultiMatch(match => match.Fields(targetFields).Query(searchRequest.QueryValue)
                            .Fuzziness(new Fuzziness("AUTO"))
                            .Type(TextQueryType.BestFields)
                        ));
                    configure.Filter(searchRequest.Filters != null && searchRequest.Filters.Any() 
                        ? queryDescriptor => queryDescriptor
                            .Terms(term => term.Field(select => select.Tags)
                                .Terms(new TermsQueryField(searchRequest.Filters.Select(FieldValue.String).ToArray()))
                            )
                        : _ => {});
                })
            );
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
            Items = searchResponse.Documents.Select(item => item.PostUuid).ToList(),
            TotalCount = searchResponse.Total,
        };
    }
}