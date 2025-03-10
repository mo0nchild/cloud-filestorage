using CloudStorage.Application.Posts.Repositories;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Application.Posts.Interfaces;

public interface ITagsService
{
    Task<TagsResult> GetOrCreateTagsAsync(IReadOnlyList<string> tagNames, IPostsRepository? postsRepository = default);
    Task<IReadOnlyList<Task>?> RemoveUnusedTags();
}
public record TagsResult(List<TagInfo> Tags, Func<Task> Rollback);