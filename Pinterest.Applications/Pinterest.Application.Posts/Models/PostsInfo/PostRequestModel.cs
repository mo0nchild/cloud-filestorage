using System.Runtime.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Commons.Models;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Core.Factories;

namespace Pinterest.Application.Posts.Models.PostsInfo;

public class PostRequestModel 
{
    public required PagedRange PagedRange { get; set; }
    public required SortingType SortingType { get; set; }
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
        RuleFor(item => item.SortingType).NotEmpty()
            .WithMessage("Please specify sorting type");
        RuleFor(item => item.TagName)
            .MustAsync(async (value, _) =>
            {
                if (value == null) return true;
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                return (await dbContext.Tags.FirstOrDefaultAsync(it => it.Name == value)) != null;
            }).WithMessage("Tag by name not found");
    }
}