using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Models.PostsInfo;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Posts.Services;

internal class PostsValidators : IPostsValidators
{
    public PostsValidators(IModelValidator<NewPostModel> newPostModelValidator,
        IModelValidator<RemovePostModel> removePostModelValidator,
        IModelValidator<PostRequestModel> postRequestModelValidator,
        IModelValidator<UpdatePostModel> updatePostModelValidator)
    {
        NewPostModelValidator = newPostModelValidator;
        PostRequestModelValidator = postRequestModelValidator;
        UpdatePostModelValidator = updatePostModelValidator;
        RemovePostModelValidator = removePostModelValidator;
    }
    public IModelValidator<PostRequestModel> PostRequestModelValidator { get; }
    public IModelValidator<UpdatePostModel> UpdatePostModelValidator { get; }
    public IModelValidator<NewPostModel> NewPostModelValidator { get; }
    public IModelValidator<RemovePostModel> RemovePostModelValidator { get; }
}