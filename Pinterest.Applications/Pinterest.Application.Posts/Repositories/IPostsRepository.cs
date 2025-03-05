using Microsoft.EntityFrameworkCore;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Repositories;

public interface IPostsRepository : IBaseRepository
{
    DbSet<PostInfo> Posts { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<TagInfo> Tags { get; set; }
}