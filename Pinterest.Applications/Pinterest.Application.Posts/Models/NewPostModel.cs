using AutoMapper;
using FluentValidation;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Core.Factories;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Models;

public class NewPostModel
{
    public string Title { get; set; } = string.Empty;
    public Guid UserUuid { get; set; } = Guid.Empty;
    public byte[] FileContent { get; set; } = [];
}

public class NewPostModelProfile : Profile
{
    public NewPostModelProfile() => CreateMap<NewPostModel, PostInfo>();
}
public class NewPostModelValidator : AbstractValidator<NewPostModel>
{
    public NewPostModelValidator(RepositoryFactoryInterface<IPostsRepository> contextFactory)
    {
        base.RuleFor(item => item.Title)
            .NotEmpty().WithMessage("Title value cannot be empty");
        base.RuleFor(item => item.FileContent)
            .NotEmpty().WithMessage("File value cannot be empty");
    }
}

