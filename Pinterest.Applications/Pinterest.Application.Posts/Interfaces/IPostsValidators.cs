using Pinterest.Application.Posts.Models.PostsInfo;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Posts.Interfaces;

public interface IPostsValidators
{
    IModelValidator<NewPostModel> NewPostModelValidator { get; }
    IModelValidator<RemovePostModel> RemovePostModelValidator { get; }
}