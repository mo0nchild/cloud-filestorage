using Microsoft.EntityFrameworkCore;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Application.Users.Repositories;

public interface IUsersRepository : IBaseRepository
{
    DbSet<User> Users { get; set; }
    DbSet<FavoritePost> FavoritePosts { get; set; }
    DbSet<Subscription> Subscriptions { get; set; }
}