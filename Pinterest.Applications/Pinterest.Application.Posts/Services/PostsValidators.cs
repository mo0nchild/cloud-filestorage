using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Models.PostsInfo;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Posts.Services;

public class PostsValidators : IPostsValidators
{
    public PostsValidators(IModelValidator<NewPostModel> newPostModelValidator,
        IModelValidator<RemovePostModel> removePostModelValidator,
        IModelValidator<PostRequestModel> postRequestModelValidator)
    {
        NewPostModelValidator = newPostModelValidator;
        PostRequestModelValidator = postRequestModelValidator;
        RemovePostModelValidator = removePostModelValidator;
    }
    public IModelValidator<PostRequestModel> PostRequestModelValidator { get; }
    public IModelValidator<NewPostModel> NewPostModelValidator { get; }
    public IModelValidator<RemovePostModel> RemovePostModelValidator { get; }
}