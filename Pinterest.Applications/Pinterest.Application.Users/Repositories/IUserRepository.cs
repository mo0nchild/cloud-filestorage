using Microsoft.EntityFrameworkCore;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Application.Users.Repositories;

public interface IUserDbContext : IBaseRepository
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserTheme> UserThemes { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<FavoritePost> FavoritePosts { get; set; }
}