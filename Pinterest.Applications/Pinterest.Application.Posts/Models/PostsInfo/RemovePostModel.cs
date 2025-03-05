using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Core.Factories;

namespace Pinterest.Application.Posts.Models.PostsInfo;

public class RemovePostModel
{
    public required Guid AuthorUuid { get; set; }
    public required Guid PostUuid { get; set; }
}

public class RemovePostModelValidator : AbstractValidator<RemovePostModel>
{
    public RemovePostModelValidator(RepositoryFactoryInterface<IPostsRepository> repositoryFactory)
    {
        RuleFor(item => item.AuthorUuid)
            .NotEmpty().WithMessage("Author Uuid is required")
            .MustAsync(async (item, value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var post = await dbContext.Posts.FirstOrDefaultAsync(it => it.Uuid == item.PostUuid);
                return post == null || post.AuthorUuid == value;
            }).WithMessage("Cannot access to Post, Author Uuid is invalid");
        RuleFor(item => item.PostUuid)
            .NotEmpty().WithMessage("Post Uuid is required")
            .MustAsync(async (value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var post = await dbContext.Posts.FirstOrDefaultAsync(it => it.Uuid == value);
                return post != null;
            }).WithMessage("Post not found by Uuid");
    }
}