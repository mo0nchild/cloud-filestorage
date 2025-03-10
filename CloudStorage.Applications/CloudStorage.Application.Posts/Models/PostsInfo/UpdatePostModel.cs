using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using CloudStorage.Application.Posts.Repositories;
using CloudStorage.Domain.CommonModels.Models;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Core.Repositories;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Application.Posts.Models.PostsInfo;

public class UpdatePostModel
{
    public required Guid AuthorUuid { get; set; }
    public required Guid PostUuid { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; } = default;
    
    public bool CommentsEnabled { get; set; } = default;
    public bool IsPublic { get; set; } = default;
    public IReadOnlyList<string> Tags { get; set; } = new List<string>();
}
public class UpdatePostModelProfile : Profile
{
    public UpdatePostModelProfile() => CreateMap<UpdatePostModel, PostInfo>()
        .ForMember(dest => dest.Tags, opt => opt.Ignore());
}
public class UpdatePostModelValidator : AbstractValidator<UpdatePostModel>
{
    public UpdatePostModelValidator(IDocumentRepository<ValidUserInfo> documentRepository, 
        RepositoryFactoryInterface<IPostsRepository> repositoryFactory)
    {
        RuleFor(item => item.Title)
            .NotEmpty().WithMessage("Title value cannot be empty");
        RuleFor(item => item.AuthorUuid)
            .NotEmpty().WithMessage("Author uuid cannot be empty")
            .MustAsync(async (value, _) =>
            {
                var user = await documentRepository.Collection.Find(it => it.UserUuid == value).FirstOrDefaultAsync();
                return user != null;
            }).WithMessage("Author uuid is invalid, user does not exist");
        RuleFor(item => item.PostUuid)
            .NotEmpty().WithMessage("Post uuid cannot be empty")
            .MustAsync(async (value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var postInfo = await dbContext.Posts.FirstOrDefaultAsync(it => it.Uuid == value);
                return postInfo != null;
            }).WithMessage("Post uuid is invalid, does not found");
        RuleFor(item => item.Tags)
            .NotEmpty().WithMessage("Tags list cannot be empty")
            .Must(item => item.All(it => it.Length > 0)).WithMessage("Tags list item cannot contain empty strings")
            .Must(item => item.All(it => it.Length <= 50))
            .WithMessage("Tags list item cannot contain more than 50 characters");
        
    }
}
