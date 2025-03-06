using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Interfaces;

public interface ITagsService
{
    Task<TagsResult> GetOrCreateTagsAsync(IReadOnlyList<string> tagNames, IPostsRepository? postsRepository = default);
    Task<IReadOnlyList<Task>?> RemoveUnusedTags();
}
public record TagsResult(List<TagInfo> Tags, Func<Task> Rollback);