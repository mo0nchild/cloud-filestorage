using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Domain.Core.Factories;

namespace CloudStorage.Application.Users.Models.FavoritePost;

public class NewFavoriteInfo
{
    public required Guid UserUuid { get; set; }
    public required Guid FavoriteUuid { get; set; }
}

public class NewFavoriteInfoValidator : AbstractValidator<NewFavoriteInfo>
{
    public NewFavoriteInfoValidator(RepositoryFactoryInterface<IUsersRepository> repositoryFactory)
    {
        RuleFor(item => item.FavoriteUuid)
            .NotEmpty().WithMessage("Favorite Uuid is required")
            .MustAsync(async (item, value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var favoriteInfo = await dbContext.FavoritePosts.Where(it => it.PostUuid == value)
                    .Include(it => it.User)
                    .FirstOrDefaultAsync(it => it.User.Uuid == item.UserUuid);
                return favoriteInfo == null;
            }).WithMessage("Favorite post already exists");
        RuleFor(item => item.UserUuid)
            .NotEmpty().WithMessage("User Uuid is required")
            .MustAsync(async (item, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(it => it.Uuid == item);
                return userInfo != null;
            }).WithMessage("User not found with this Uuid");
    }
}