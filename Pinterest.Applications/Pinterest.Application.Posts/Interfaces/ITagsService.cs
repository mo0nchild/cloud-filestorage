using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Interfaces;

public interface ITagsService
{
    Task<(List<TagInfo> Tags, Action Rollback)> GetOrCreateTagsAsync(IReadOnlyList<string> tagNames, 
        IPostsRepository? postsRepository = default);
    Task RemoveUnusedTags(IReadOnlyList<Guid> affectedTags, IPostsRepository? repository = default);
}