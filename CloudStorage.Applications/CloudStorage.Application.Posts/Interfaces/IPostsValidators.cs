using CloudStorage.Application.Posts.Models.PostsInfo;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Posts.Interfaces;

public interface IPostsValidators
{
    IModelValidator<NewPostModel> NewPostModelValidator { get; }
    IModelValidator<RemovePostModel> RemovePostModelValidator { get; }
    IModelValidator<UpdatePostModel> UpdatePostModelValidator { get; }
    IModelValidator<PostRequestModel> PostRequestModelValidator { get; }
}