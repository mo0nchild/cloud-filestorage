using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Database.Posts.Configurations;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Database.Posts.Contexts;

public class PostsDbContext : DbContext, IPostsRepository
{
    public DbSet<PostInfo> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TagInfo> Tags { get; set; }
    public PostsDbContext(DbContextOptions<PostsDbContext> options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PostsConfiguration());
        modelBuilder.ApplyConfiguration(new CommentsConfiguration());
    }
    public Task<IDbContextTransaction> BeginTransactionAsync() => Database.BeginTransactionAsync();
    public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();
}