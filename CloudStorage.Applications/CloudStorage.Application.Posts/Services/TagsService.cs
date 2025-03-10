using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Posts.Infrastructures.Interfaces;
using CloudStorage.Application.Posts.Infrastructures.Models;
using CloudStorage.Application.Posts.Interfaces;
using CloudStorage.Application.Posts.Repositories;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Application.Posts.Services;

internal class TagsService : ITagsService
{
    private readonly RepositoryFactoryInterface<IPostsRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly ISearchEngine<TagIndex> _searchEngine;
    private static readonly int TagsPerChunk = 5;
    
    public TagsService(RepositoryFactoryInterface<IPostsRepository> repositoryFactory,
        IMapper mapper,
        ISearchEngine<TagIndex> searchEngine,
        ILogger<TagsService> logger)
    {
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
        _searchEngine = searchEngine;
        Logger = logger;
    }
    private ILogger<TagsService> Logger { get; }
    
    public async Task<TagsResult> GetOrCreateTagsAsync(IReadOnlyList<string> tagNames, 
        IPostsRepository? repository = null)
    {
        var dbContext = repository ?? await _repositoryFactory.CreateRepositoryAsync();
        await using var transaction = repository == null ? await dbContext.BeginTransactionAsync() : null;
        try {
            var distinctTagNames = tagNames.Distinct().ToList();
            var existingTags = await dbContext.Tags.Where(tag => distinctTagNames.Contains(tag.Name))
                .ToDictionaryAsync(tag => tag.Name);

            var newTags = distinctTagNames.Where(tagName => !existingTags.ContainsKey(tagName))
                .Select(tagName => new TagInfo { Name = tagName }).ToList();
            if (newTags.Count > 0)
            {
                await dbContext.Tags.AddRangeAsync(newTags);
                await dbContext.SaveChangesAsync();
                foreach (var newTag in newTags)
                {
                    await _searchEngine.IndexPostAsync(_mapper.Map<TagIndex>(newTag));
                }
            }
            if (transaction != null) await transaction.CommitAsync();
            return new TagsResult(existingTags.Values.Concat(newTags).ToList(), async () =>
            {
                foreach (var newTag in newTags) await _searchEngine.RemovePostAsync(newTag.Uuid);
            });
        }
        catch {
            if (transaction != null) await transaction.RollbackAsync(); 
            throw;
        }
        finally { if (repository == null) dbContext.Dispose(); }
    }
    public async Task<IReadOnlyList<Task>?> RemoveUnusedTags()
    {
        var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var unusedTags = await dbContext.Tags.GroupJoin(
                dbContext.Posts.SelectMany(post => post.Tags),
                tag => tag.Uuid, postTag => postTag.Uuid,
                (tag, postTagGroup) => new { tag, postTagGroup })
            .SelectMany(item => item.postTagGroup.DefaultIfEmpty(), (it, postTag) => new { it.tag, postTag })
            .Where(item => item.postTag == null)
            .Select(item => item.tag).ToListAsync();
        
        if (unusedTags.Count <= 0) return null;
        return unusedTags.Chunk(TagsPerChunk).Select(RemoveTagsAsync).ToList();
    }
    private async Task RemoveTagsAsync(IReadOnlyList<TagInfo> unusedTags)
    {
        var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var rollbackActions = new List<Func<Task>>();
        await using var transaction = await dbContext.BeginTransactionAsync();
        try {
            dbContext.Tags.RemoveRange(unusedTags);
            await dbContext.SaveChangesAsync();

            foreach (var tag in unusedTags)
            {
                await _searchEngine.RemovePostAsync(tag.Uuid);
                rollbackActions.Add(async () => await _searchEngine.IndexPostAsync(_mapper.Map<TagIndex>(tag)));
            }
            await transaction.CommitAsync();
        }
        catch {
            await transaction.RollbackAsync(); 
            foreach (var item in rollbackActions) await item.Invoke();
            throw;
        }
    }
}