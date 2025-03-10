using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CloudStorage.Application.Posts.Repositories;
using CloudStorage.Database.Posts.Configurations;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Database.Posts.Contexts;

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
        modelBuilder.ApplyConfiguration(new TagsConfiguration());
    }
    public Task<IDbContextTransaction> BeginTransactionAsync() => Database.BeginTransactionAsync();
    public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();
}