using Microsoft.EntityFrameworkCore;
using CloudStorage.Domain.Core.Repositories;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Application.Posts.Repositories;

public interface IPostsRepository : IBaseRepository
{
    DbSet<PostInfo> Posts { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<TagInfo> Tags { get; set; }
}