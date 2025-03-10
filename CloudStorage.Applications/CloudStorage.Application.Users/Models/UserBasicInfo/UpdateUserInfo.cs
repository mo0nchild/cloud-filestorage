using FluentValidation;
using MongoDB.Driver.Linq;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Domain.Core.Factories;

namespace CloudStorage.Application.Users.Models.UserBasicInfo;

public class UpdateUserInfo
{
    public required Guid UserUuid { get; set; }
    public required string Username { get; set; }
    public IReadOnlyList<string> UserThemes { get; set; } = new List<string>();
}

public class UpdateUserInfoValidator : AbstractValidator<UpdateUserInfo>
{
    public UpdateUserInfoValidator(RepositoryFactoryInterface<IUsersRepository> repositoryFactory)
    {
        RuleFor(item => item.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters")
            .MustAsync(async (@object, value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var user = await dbContext.Users.FirstOrDefaultAsync(it => it.Username == value
                                                                           && it.Uuid != @object.UserUuid);
                return user == null;
            }).WithMessage("Username already exists");
        RuleFor(item => item.UserUuid)
            .NotEmpty().WithMessage("UserUuid is required");
        RuleFor(item => item.UserThemes)
            .Must(item => item.All(it => it.Length > 0)).WithMessage("Tags list item cannot contain empty strings");
    }
}