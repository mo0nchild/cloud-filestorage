using Pinterest.Application.Commons.Models;
using Pinterest.Application.Posts.Models;
using Pinterest.Application.Posts.Models.PostsInfo;

namespace Pinterest.Application.Posts.Interfaces;

public interface IPostsService
{
    Task<PagedResult<PostModel>> GetPostsListAsync(PostRequestModel requestModel);
    Task<PagedResult<PostModel>> FindPostsByQueryAsync(string queryValue);
    
    Task AddPostAsync(NewPostModel newPost);
    Task DeletePostAsync(RemovePostModel removePost);
}