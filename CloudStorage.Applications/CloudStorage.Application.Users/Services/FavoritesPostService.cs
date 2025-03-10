using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Users.Interfaces;
using CloudStorage.Application.Users.Models.FavoritePost;
using CloudStorage.Application.Users.Models.FavoritePostInfo;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Domain.CommonModels.Models;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Core.Repositories;
using CloudStorage.Domain.Users.Entities;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Users.Services;

using EFCoreExtensions = EntityFrameworkQueryableExtensions;

internal class FavoritesPostService : IFavoritesPostService
{
    private readonly RepositoryFactoryInterface<IUsersRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly IModelValidator<NewFavoriteInfo> _newFavoriteValidator;
    private readonly IDocumentRepository<ValidPostInfo> _documentRepository;

    public FavoritesPostService(RepositoryFactoryInterface<IUsersRepository> repositoryFactory,
        IMapper mapper,
        IModelValidator<NewFavoriteInfo> newFavoriteValidator, 
        IDocumentRepository<ValidPostInfo> documentRepository,
        ILogger<FavoritesPostService> logger)
    {
        Logger = logger;
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
        _newFavoriteValidator = newFavoriteValidator;
        _documentRepository = documentRepository;
    }
    private ILogger<FavoritesPostService> Logger { get; }
    
    public async Task AddFavorite(NewFavoriteInfo newFavorite)
    {
        await _newFavoriteValidator.CheckAsync(newFavorite);
        var validPost = await _documentRepository.Collection.Find(item => item.PostUuid == newFavorite.FavoriteUuid)
            .FirstOrDefaultAsync();
        if (validPost == null) throw new ProcessException("Post Uuid not valid (found)");
        
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var userInfo = await MongoQueryable.FirstOrDefaultAsync(dbContext.Users, item => item.Uuid == newFavorite.UserUuid);
        
        await dbContext.FavoritePosts.AddRangeAsync(new FavoritePost()
        {
            PostUuid = validPost.PostUuid,
            User = userInfo,
        });
        await dbContext.SaveChangesAsync();
    }
    public async Task RemoveFavorite(RemoveFavoritePost removeInfo)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var query = dbContext.FavoritePosts.Where(item => item.PostUuid == removeInfo.FavoriteUuid)
            .Include(item => item.User);
        
        var favoritePost = await EFCoreExtensions.FirstOrDefaultAsync(query, item => item.User.Uuid == removeInfo.UserUuid);
        if (favoritePost == null) throw new ProcessException("Favorite post not found");
        dbContext.FavoritePosts.Remove(favoritePost);
        
        await dbContext.SaveChangesAsync();
    }
    public async Task<FavoriteInfo> GetFavoritesList(Guid userUuid)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var query = dbContext.Users.Where(item => item.Uuid == userUuid).Include(item => item.FavoritesPosts);
        
        var userInfo = await EFCoreExtensions.FirstOrDefaultAsync(query);
        if (userInfo == null) throw new ProcessException("User by Uuid not found");
        return _mapper.Map<FavoriteInfo>(userInfo.FavoritesPosts);
    }
}