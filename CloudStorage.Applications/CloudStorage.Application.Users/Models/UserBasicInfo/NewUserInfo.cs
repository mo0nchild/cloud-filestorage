using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Messages.SagaMessages.CreateAccountSaga;
using CloudStorage.Domain.Users.Entities;

namespace CloudStorage.Application.Users.Models.UserBasicInfo;

public class NewUserInfo
{
    public required string Username { get; set; }
    public required string Email { get; set; }
}

public class NewUserInfoProfile : Profile
{
    public NewUserInfoProfile()
    {
        CreateMap<NewUserInfo, User>()
            .ForMember(u => u.Username, u => u.MapFrom(it => it.Username))
            .ForMember(u => u.Email, u => u.MapFrom(it => it.Email));
        CreateMap<CreateUserPayload, NewUserInfo>();
    }
}
public class NewUserInfoValidator : AbstractValidator<NewUserInfo>
{
    public NewUserInfoValidator(RepositoryFactoryInterface<IUsersRepository> repositoryFactory)
    {
        RuleFor(item => item.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters")
            .MustAsync(async (value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var user = await dbContext.Users.FirstOrDefaultAsync(it => it.Username == value);
                return user == null;
            }).WithMessage("Username already exists");
        RuleFor(item => item.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(100).WithMessage("The mail value cannot exceed 100 characters")
            .EmailAddress().WithMessage("Email is not a valid email address")
            .MustAsync(async (value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var user = await dbContext.Users.FirstOrDefaultAsync(it => it.Email == value);
                return user == null;
            }).WithMessage("Email already exists");
    }
}