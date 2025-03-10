using System.Data;
using AutoMapper;
using FluentValidation;
using MongoDB.Driver;
using CloudStorage.Application.Posts.Repositories;
using CloudStorage.Domain.CommonModels.Models;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Core.Repositories;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Application.Posts.Models.PostsInfo;

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
    public NewPostModelProfile() => CreateMap<NewPostModel, PostInfo>()
        .ForMember(dest => dest.Tags, opt => opt.Ignore());
}
public class NewPostModelValidator : AbstractValidator<NewPostModel>
{
    public NewPostModelValidator(IDocumentRepository<ValidUserInfo> documentRepository)
    {
        RuleFor(item => item.Title)
            .NotEmpty().WithMessage("Title value cannot be empty");
        RuleFor(item => item.FileUuid)
            .NotEmpty().WithMessage("File uuid cannot be empty");
        RuleFor(item => item.AuthorUuid)
            .NotEmpty().WithMessage("Author uuid cannot be empty")
            .MustAsync(async (value, _) =>
            {
                var user = await documentRepository.Collection.Find(it => it.UserUuid == value).FirstOrDefaultAsync();
                return user != null;
            }).WithMessage("Author uuid is invalid, user does not exist");
        RuleFor(item => item.Tags)
            .NotEmpty().WithMessage("Tags list cannot be empty")
            .Must(item => item.All(it => it.Length > 0)).WithMessage("Tags list item cannot contain empty strings")
            .Must(item => item.All(it => it.Length <= 50))
            .WithMessage("Tags list item cannot contain more than 50 characters");
        
    }
}

