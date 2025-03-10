using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Commons.Helpers;
using CloudStorage.Application.Commons.Models;
using CloudStorage.Application.Posts.Infrastructures;
using CloudStorage.Application.Posts.Infrastructures.Interfaces;
using CloudStorage.Application.Posts.Infrastructures.Models;
using CloudStorage.Application.Posts.Interfaces;
using CloudStorage.Application.Posts.Models;
using CloudStorage.Application.Posts.Models.PostsInfo;
using CloudStorage.Application.Posts.Repositories;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.PostsMessages;
using CloudStorage.Domain.Posts.Entities;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Posts.Services;

internal class PostsService : IPostsService
{
    private readonly RepositoryFactoryInterface<IPostsRepository> _repositoryFactory;
    private readonly InnerTransactionProcessor _transactionProcessor;
    private readonly IMapper _mapper;
    private readonly ISearchEngine<PostIndex> _searchEngine;
    private readonly IPostsValidators _validators;
    private readonly ITagsService _tagsService;
    private readonly IMessageProducer _messagesProducer;

    public PostsService(RepositoryFactoryInterface<IPostsRepository> repositoryFactory,
        [FromKeyedServices("PostsTransactions")] InnerTransactionProcessor transactionProcessor,
        IMapper mapper,
        ISearchEngine<PostIndex> searchEngine,
        IPostsValidators validators,
        ITagsService tagsService,
        IMessageProducer messagesProducer,
        ILogger<PostsService> logger)
    {
        _repositoryFactory = repositoryFactory;
        _transactionProcessor = transactionProcessor;
        _mapper = mapper;
        _searchEngine = searchEngine;
        _validators = validators;
        _tagsService = tagsService;
        _messagesProducer = messagesProducer;
        Logger = logger;
    }
    private ILogger<PostsService> Logger { get; }
    
    public async Task<PagedResult<PostModel>> GetPostsListAsync(PostRequestModel requestModel)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var query = dbContext.Posts.AsQueryable();
        
        if (!string.IsNullOrEmpty(requestModel.TagName))
        {
            query = query.Where(post => post.Tags.Any(tag => tag.Name == requestModel.TagName));
        }
        query = requestModel.SortingType switch
        {
            SortingType.ByRatingAscending => query.OrderBy(post => post.LikesCount),
            SortingType.ByRatingDescending => query.OrderByDescending(post => post.LikesCount),
            SortingType.ByDateAscending => query.OrderBy(post => post.CreatedTime),
            SortingType.ByDateDescending => query.OrderByDescending(post => post.CreatedTime),
            SortingType.ByViewsCountAscending => query.OrderBy(post => post.ViewsCount),
            SortingType.ByViewsCountDescending => query.OrderByDescending(post => post.ViewsCount),
            _ => throw new ProcessException($"Invalid sorting type: {requestModel.SortingType}")
        };
        var totalCount = await query.CountAsync();
        var posts = await query.Skip(requestModel.PagedRange.From)
            .Take(requestModel.PagedRange.ListSize)
            .ToListAsync();
        
        return new PagedResult<PostModel>
        {
            Items = _mapper.Map<List<PostModel>>(posts),
            TotalCount = totalCount,
        };
    }
    public Task<PagedResult<PostModel>> FindPostsByQueryAsync(string queryValue)
    {
        throw new NotImplementedException();
    }
    public async Task UpdatePostAsync(UpdatePostModel updatePost)
    {
        await _validators.UpdatePostModelValidator.CheckAsync(updatePost);
        using var innerTransaction = await _transactionProcessor.BeginInnerTransaction(updatePost.PostUuid);
        
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        Func<Task>? innerRollback = null;
        await using var transaction = await dbContext.BeginTransactionAsync();
        try {
            var postInfo = await dbContext.Posts.FirstAsync(it => it.Uuid == updatePost.PostUuid);
            postInfo.Title = updatePost.Title;
            postInfo.Description = updatePost.Description;
            postInfo.CommentsEnabled = updatePost.CommentsEnabled;
            postInfo.IsPublic = updatePost.IsPublic;
            
            var (tags, tagsRollback) = await _tagsService.GetOrCreateTagsAsync(updatePost.Tags.ToList(), dbContext);
            (postInfo.Tags, innerRollback) = (tags, tagsRollback);
            foreach (var tag in tags) tag.Posts.Add(postInfo);
            
            dbContext.Posts.Update(postInfo);
            await dbContext.SaveChangesAsync();
            
            await _searchEngine.UpdatePostAsync(_mapper.Map<PostIndex>(postInfo));
            await transaction.CommitAsync();
        }
        catch (Exception error)
        {
            await transaction.RollbackAsync();
            if (innerRollback != null) await innerRollback.Invoke();
            
            Logger.LogError($"Error with updating exists post - {updatePost.PostUuid}: {error.Message}"); 
            throw;
        }
    }
    public async Task AddPostAsync(NewPostModel newPost)
    {
        await _validators.NewPostModelValidator.CheckAsync(newPost);
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        Func<Task>? innerRollback = null;
        await using var transaction = await dbContext.BeginTransactionAsync();
        try {
            var mappedPost = _mapper.Map<PostInfo>(newPost);
            var (tags, tagsRollback) = await _tagsService.GetOrCreateTagsAsync(newPost.Tags.ToList(), dbContext);
            
            (mappedPost.Tags, innerRollback) = (tags, tagsRollback);
            foreach (var tag in tags) tag.Posts.Add(mappedPost);
            
            await dbContext.Posts.AddAsync(mappedPost);
            await dbContext.SaveChangesAsync();
            
            await _searchEngine.IndexPostAsync(_mapper.Map<PostIndex>(mappedPost));
            await transaction.CommitAsync();
            await _messagesProducer.SendToAllAsync(CreatedPostMessage.RoutingPath, new CreatedPostMessage()
            {
                PostUuid = mappedPost.Uuid
            });
        }
        catch (Exception error)
        {
            await transaction.RollbackAsync();
            if (innerRollback != null) await innerRollback.Invoke();
            
            Logger.LogError($"Error with adding new post - {newPost.Title}: {error.Message}"); 
            throw;
        }
    }
    public async Task DeletePostAsync(RemovePostModel removePost)
    {
        await _validators.RemovePostModelValidator.CheckAsync(removePost);
        using var innerTransaction = await _transactionProcessor.BeginInnerTransaction(removePost.PostUuid);
        
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        await using var transaction = await dbContext.BeginTransactionAsync();
        try {
            var postInfo = await dbContext.Posts.Include(it => it.Tags).FirstAsync(it => it.Uuid == removePost.PostUuid);
            var affectedTags = postInfo.Tags.Select(item => item.Uuid).ToList();

            dbContext.Posts.Remove(postInfo);
            await dbContext.SaveChangesAsync();
            await _searchEngine.RemovePostAsync(postInfo.Uuid);

            await transaction.CommitAsync();
            await _messagesProducer.SendToAllAsync(RemovePostMessage.RoutingPath, new RemovePostMessage()
            {
                PostUuid = removePost.PostUuid
            });
        }
        catch (Exception error)
        {
            await transaction.RollbackAsync();
            Logger.LogError($"Error with removing post - {removePost.PostUuid}: {error.Message}"); 
            throw;
        }
    }
}