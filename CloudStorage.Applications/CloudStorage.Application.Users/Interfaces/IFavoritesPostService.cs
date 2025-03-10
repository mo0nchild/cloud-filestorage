using CloudStorage.Application.Users.Models;
using CloudStorage.Application.Users.Models.FavoritePost;
using CloudStorage.Application.Users.Models.FavoritePostInfo;
using CloudStorage.Domain.Users.Entities;

namespace CloudStorage.Application.Users.Interfaces;

public interface IFavoritesPostService
{
    Task AddFavorite(NewFavoriteInfo newFavorite);
    Task RemoveFavorite(RemoveFavoritePost removeInfo);

    Task<FavoriteInfo> GetFavoritesList(Guid userUuid);
}