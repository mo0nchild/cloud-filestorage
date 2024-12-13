using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Accounts.Repositories;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Database.Posts.Configurations;
using Pinterest.Domain.Authorization.Entities;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Database.Posts.Contexts;

public class PostsDbContext : DbContext, IPostsRepository
{
    public DbSet<PostInfo> Posts { get; set; }
    public PostsDbContext(DbContextOptions<PostsDbContext> options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PostsConfiguration());
    }
}