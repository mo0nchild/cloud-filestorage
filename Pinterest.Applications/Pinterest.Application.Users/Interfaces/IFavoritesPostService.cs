using Pinterest.Application.Users.Models;
using Pinterest.Application.Users.Models.FavoritePost;
using Pinterest.Application.Users.Models.FavoritePostInfo;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Application.Users.Interfaces;

public interface IFavoritesPostService
{
    Task AddFavorite(NewFavoriteInfo newFavorite);
    Task RemoveFavorite(RemoveFavoritePost removeInfo);

    Task<FavoriteInfo> GetFavoritesList(Guid userUuid);
}