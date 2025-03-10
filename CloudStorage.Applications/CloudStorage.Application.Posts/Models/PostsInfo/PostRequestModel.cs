using System.Runtime.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CloudStorage.Application.Commons.Models;
using CloudStorage.Application.Posts.Repositories;
using CloudStorage.Domain.Core.Factories;

namespace CloudStorage.Application.Posts.Models.PostsInfo;

public class PostRequestModel 
{
    public required PagedRange PagedRange { get; set; }
    public SortingType SortingType { get; set; } = SortingType.ByRatingDescending;
    public string? TagName { get; set; } = default;
}
public enum SortingType
{
    ByRatingAscending,
    ByRatingDescending,
    ByDateAscending, 
    ByDateDescending,
    ByViewsCountAscending,
    ByViewsCountDescending,
}
public class PostRequestModelValidator : AbstractValidator<PostRequestModel>
{
    public PostRequestModelValidator(RepositoryFactoryInterface<IPostsRepository> repositoryFactory)
    {
        RuleFor(item => item.PagedRange).SetValidator(new PagedRangeValidator());
        RuleFor(item => item.TagName)
            .MustAsync(async (value, _) =>
            {
                if (value == null) return true;
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                return (await dbContext.Tags.FirstOrDefaultAsync(it => it.Name == value)) != null;
            }).WithMessage("Tag by name not found");
    }
}