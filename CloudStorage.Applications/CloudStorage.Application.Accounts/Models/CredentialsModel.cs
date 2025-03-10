using AutoMapper;
using FluentValidation;
using CloudStorage.Application.Accounts.Repositories;
using CloudStorage.Domain.Authorization.Entities;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Messages.SagaMessages.CreateAccountSaga;

namespace CloudStorage.Application.Accounts.Models;

using BCryptType = BCrypt.Net.BCrypt;

public class CredentialsModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CredentialsModelProfile : Profile
{
    public CredentialsModelProfile()
    {
        CreateMap<AccountInfo, CredentialsModel>();
        CreateMap<CredentialsModel, AccountInfo>()
            .ForMember(item => item.Email, options => options.MapFrom(p => p.Email))
            .ForMember(item => item.Password, options =>
            {
                options.MapFrom(p => BCryptType.HashPassword(p.Password));
            })
            .ForMember(item => item.Uuid, options => options.MapFrom(p => Guid.NewGuid()));
        CreateMap<CreateAccountPayload, CredentialsModel>();
    }
}

public class CredentialsModelValidator : AbstractValidator<CredentialsModel>
{
    public CredentialsModelValidator(RepositoryFactoryInterface<IAccountsRepository> contextFactory)
    {
        RuleFor(item => item.Email)
            .NotEmpty().WithMessage("The mail value cannot be empty")
            .MaximumLength(100).WithMessage("The mail value cannot exceed 100 characters")
            .EmailAddress().WithMessage("Invalid mail format")
            .Must(item =>
            {
                using var context = contextFactory.CreateRepository();
                var profile = context.AccountInfos.FirstOrDefault(p => p.Email == item);
                return profile == null;
            }).WithMessage("User is already registered");
        RuleFor(item => item.Password)
            .NotEmpty().WithMessage("The password value cannot be empty");
    }
}