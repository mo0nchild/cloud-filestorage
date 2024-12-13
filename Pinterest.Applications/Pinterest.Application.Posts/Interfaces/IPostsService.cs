using Pinterest.Application.Posts.Models;

namespace Pinterest.Application.Posts.Interfaces;

public interface IPostsService
{
    public Task<List<PostModel>> GetPostsAsync(Guid userUuid);
    public Task AddPostAsync(NewPostModel newPost);
}