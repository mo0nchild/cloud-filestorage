using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Posts.Infrastructures.Interfaces;
using Pinterest.Application.Posts.Infrastructures.Models;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Core.Factories;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Services;

internal class TagsService : ITagsService
{
    private readonly RepositoryFactoryInterface<IPostsRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly ISearchEngine<TagIndex> _searchEngine;

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
    
    public async Task<(List<TagInfo> Tags, Action Rollback)> GetOrCreateTagsAsync(IReadOnlyList<string> tagNames, 
        IPostsRepository? repository = null)
    {
        var dbContext = repository ?? await _repositoryFactory.CreateRepositoryAsync();
        await using var transaction = repository == null ? await dbContext.BeginTransactionAsync() : null;
        try
        {
            var distinctTagNames = tagNames.Distinct().ToList();
            var existingTags = await dbContext.Tags.Where(tag => distinctTagNames.Contains(tag.Name))
                .ToDictionaryAsync(tag => tag.Name);

            var newTags = distinctTagNames.Where(tagName => !existingTags.ContainsKey(tagName))
                .Select(tagName => new TagInfo { Name = tagName }).ToList();
            if (newTags.Any())
            {
                await dbContext.Tags.AddRangeAsync(newTags);
                await dbContext.SaveChangesAsync();
                foreach (var newTag in newTags)
                {
                    await _searchEngine.IndexPostAsync(_mapper.Map<TagIndex>(newTag));
                }
            }
            if (transaction != null) await transaction.CommitAsync();
            return (existingTags.Values.Concat(newTags).ToList(), delegate()
            {
                foreach (var newTag in newTags) _searchEngine.RemovePostAsync(newTag.Uuid);
            });
        }
        catch {
            if (transaction != null) await transaction.RollbackAsync(); 
            throw;
        }
        finally { if (repository == null) dbContext.Dispose(); }
    }
    public async Task RemoveUnusedTags(IReadOnlyList<Guid> affectedTags, IPostsRepository? repository = null)
    {
        var dbContext = repository ?? await _repositoryFactory.CreateRepositoryAsync();
        await using var transaction = repository == null ? await dbContext.BeginTransactionAsync() : null;
        try
        {
            var unusedTags = await dbContext.Tags.Where(tag => affectedTags.Contains(tag.Uuid))
                .GroupJoin(
                    dbContext.Posts.SelectMany(post => post.Tags),
                    tag => tag.Uuid, postTag => postTag.Uuid,
                    (tag, postTagGroup) => new { tag, postTagGroup })
                .SelectMany(item => item.postTagGroup.DefaultIfEmpty(), (it, postTag) => new { it.tag, postTag })
                .Where(item => item.postTag == null)
                .Select(item => item.tag).ToListAsync();
            
            if (!unusedTags.Any()) return;
            dbContext.Tags.RemoveRange(unusedTags);
            await dbContext.SaveChangesAsync();

            foreach (var tag in unusedTags) await _searchEngine.RemovePostAsync(tag.Uuid);
            if (transaction != null) await transaction.CommitAsync();
        }
        catch {
            if (transaction != null) await transaction.RollbackAsync(); 
            throw;
        }
    }
}