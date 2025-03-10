using CloudStorage.Application.Commons.Models;
using CloudStorage.Application.Posts.Models;
using CloudStorage.Application.Posts.Models.PostsInfo;

namespace CloudStorage.Application.Posts.Interfaces;

public interface IPostsService
{
    Task<PagedResult<PostModel>> GetPostsListAsync(PostRequestModel requestModel);
    Task<PagedResult<PostModel>> FindPostsByQueryAsync(string queryValue);

    Task UpdatePostAsync(UpdatePostModel updatePost);
    Task AddPostAsync(NewPostModel newPost);
    Task DeletePostAsync(RemovePostModel removePost);
}