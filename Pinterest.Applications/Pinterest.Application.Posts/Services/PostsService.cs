using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Commons.Models;
using Pinterest.Application.Posts.Infrastructures;
using Pinterest.Application.Posts.Infrastructures.Interfaces;
using Pinterest.Application.Posts.Infrastructures.Models;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Models;
using Pinterest.Application.Posts.Models.PostsInfo;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Core.Factories;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages.PostsMessages;
using Pinterest.Domain.Posts.Entities;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Posts.Services;

internal class PostsService : IPostsService
{
    private readonly RepositoryFactoryInterface<IPostsRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly ISearchEngine<PostIndex> _searchEngine;
    private readonly IPostsValidators _validators;
    private readonly ITagsService _tagsService;
    private readonly IMessageProducer _messagesProducer;

    public PostsService(RepositoryFactoryInterface<IPostsRepository> repositoryFactory,
        IMapper mapper,
        ISearchEngine<PostIndex> searchEngine,
        IPostsValidators validators,
        ITagsService tagsService,
        IMessageProducer messagesProducer,
        ILogger<PostsService> logger)
    {
        _repositoryFactory = repositoryFactory;
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
        throw new NotImplementedException();
    }
    public Task<PagedResult<PostModel>> FindPostsByQueryAsync(string queryValue)
    {
        throw new NotImplementedException();
    }
    public async Task AddPostAsync(NewPostModel newPost)
    {
        await _validators.NewPostModelValidator.CheckAsync(newPost);
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        Action? innerRollback = null;
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
            if (innerRollback != null) innerRollback.Invoke();
            
            Logger.LogError($"Error with adding new post - {newPost.Title}: {error.Message}"); 
            throw;
        }
    }
    public async Task DeletePostAsync(RemovePostModel removePost)
    {
        await _validators.RemovePostModelValidator.CheckAsync(removePost);
        
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        await using var transaction = await dbContext.BeginTransactionAsync();
        try
        {
            var postInfo = await dbContext.Posts.Include(it => it.Tags).FirstAsync(it => it.Uuid == removePost.PostUuid);
            var affectedTags = postInfo.Tags.Select(item => item.Uuid).ToList();

            dbContext.Posts.Remove(postInfo);
            await dbContext.SaveChangesAsync();
            await _searchEngine.RemovePostAsync(postInfo.Uuid);
            await _tagsService.RemoveUnusedTags(affectedTags, dbContext);

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