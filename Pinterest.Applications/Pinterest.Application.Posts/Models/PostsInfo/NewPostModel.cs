using System.Data;
using AutoMapper;
using FluentValidation;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Core.Factories;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Models.PostsInfo;

public class NewPostModel
{
    public required Guid AuthorUuid { get; set; }
    public required string Title { get; set; }
    public required Guid FileUuid { get; set; }
    public Guid? PreviewUuid { get; set; } = default;
    public string? Description { get; set; } = default;
    
    public bool CommentsEnabled { get; set; } = default;
    public bool IsPublic { get; set; } = default;
    public IReadOnlyList<string> Tags { get; set; } = new List<string>();
}

public class NewPostModelProfile : Profile
{
    public NewPostModelProfile() => CreateMap<NewPostModel, PostInfo>();
}
public class NewPostModelValidator : AbstractValidator<NewPostModel>
{
    public NewPostModelValidator(RepositoryFactoryInterface<IPostsRepository> contextFactory)
    {
        RuleFor(item => item.Title)
            .NotEmpty().WithMessage("Title value cannot be empty");
        RuleFor(item => item.FileUuid)
            .NotEmpty().WithMessage("File uuid cannot be empty");
        RuleFor(item => item.AuthorUuid)
            .NotEmpty().WithMessage("Author uuid cannot be empty");
        RuleFor(item => item.Tags)
            .NotEmpty().WithMessage("Tags list cannot be empty")
            .Must(item => item.All(it => it.Length > 0)).WithMessage("Tags list item cannot contain empty strings");
        
    }
}

