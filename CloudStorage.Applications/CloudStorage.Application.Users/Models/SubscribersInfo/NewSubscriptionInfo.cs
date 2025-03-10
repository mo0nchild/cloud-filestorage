using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Domain.Core.Factories;

namespace CloudStorage.Application.Users.Models.SubscribersInfo;

public class NewSubscriptionInfo
{
    public required Guid UserUuid { get; set; }
    public required Guid AuthorUuid { get; set; }
}

public class NewSubscriptionInfoValidator : AbstractValidator<NewSubscriptionInfo>
{
    public NewSubscriptionInfoValidator(RepositoryFactoryInterface<IUsersRepository> repositoryFactory)
    {
        RuleFor(item => item.UserUuid)
            .NotEmpty().WithMessage("UserUuid is required")
            .MustAsync(async (item, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(it => it.Uuid == item);
                return userInfo != null;
            }).WithMessage("User not found by Uuid")
            .MustAsync(async (item, value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var record = await dbContext.Subscriptions.Include(it => it.Subscriber)
                    .Include(it => it.Author)
                    .FirstOrDefaultAsync(it => it.Subscriber.Uuid == item.UserUuid && it.Author.Uuid == item.AuthorUuid);
                return record == null;
            }).WithMessage("Subscription already exists");
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