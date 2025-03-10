using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Domain.Core.Factories;

namespace CloudStorage.Application.Users.Models.SubscribersInfo;

public class RemoveSubscription
{
    public required Guid UserUuid { get; set; }
    public required Guid AuthorUuid { get; set; }
}

public class RemoveSubscriptionValidator : AbstractValidator<RemoveSubscription>
{
    public RemoveSubscriptionValidator(RepositoryFactoryInterface<IUsersRepository> repositoryFactory)
    {
        RuleFor(item => item.UserUuid)
            .NotEmpty().WithMessage("UserUuid is required")
            .MustAsync(async (item, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(it => it.Uuid == item);
                return userInfo != null;
            }).WithMessage("User not found by Uuid");
        RuleFor(item => item.AuthorUuid)
            .NotEmpty().WithMessage("AuthorUuid is required")
            .MustAsync(async (item, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(it => it.Uuid == item);
                return userInfo != null;
            }).WithMessage("Author not found by Uuid");
    }
}