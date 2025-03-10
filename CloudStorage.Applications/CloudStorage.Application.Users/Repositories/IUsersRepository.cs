using Microsoft.EntityFrameworkCore;
using CloudStorage.Domain.Core.Repositories;
using CloudStorage.Domain.Users.Entities;

namespace CloudStorage.Application.Users.Repositories;

public interface IUsersRepository : IBaseRepository
{
    DbSet<User> Users { get; set; }
    DbSet<FavoritePost> FavoritePosts { get; set; }
    DbSet<Subscription> Subscriptions { get; set; }
}