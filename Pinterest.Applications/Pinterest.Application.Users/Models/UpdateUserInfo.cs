using FluentValidation;
using MongoDB.Driver.Linq;
using Pinterest.Application.Users.Repositories;
using Pinterest.Domain.Core.Factories;

namespace Pinterest.Application.Users.Models;

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
            .MustAsync(async (@object, value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var user = await dbContext.Users.FirstOrDefaultAsync(it => it.Username == value
                                                                           && it.Uuid != @object.UserUuid);
                return user == null;
            }).WithMessage("Username already exists");
        RuleFor(item => item.UserUuid)
            .NotEmpty().WithMessage("UserUuid is required");
    }
}