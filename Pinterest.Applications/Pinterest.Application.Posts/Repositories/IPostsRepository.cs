using Microsoft.EntityFrameworkCore;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Repositories;

public interface IPostsRepository : IBaseRepository
{
    public DbSet<PostInfo> Posts { get; set; }
}