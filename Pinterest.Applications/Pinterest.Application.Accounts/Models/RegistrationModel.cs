using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Accounts.Repositories;
using Pinterest.Domain.Authorization.Entities;
using Pinterest.Domain.Core.Factories;

namespace Pinterest.Application.Accounts.Models;

using BCryptType = BCrypt.Net.BCrypt;
public class RegistrationModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class RegistrationModelProfile : Profile
{
    public RegistrationModelProfile()
    {
        CreateMap<RegistrationModel, AccountInfo>()
            .ForMember(item => item.Email, options => options.MapFrom(p => p.Email))
            .ForMember(item => item.Password, options =>
            {
                options.MapFrom(p => BCryptType.HashPassword(p.Password));
            })
            .ForMember(item => item.Uuid, options => options.MapFrom(p => Guid.NewGuid()));
    }
}
public class RegistrationModelValidator : AbstractValidator<RegistrationModel>
{
    public RegistrationModelValidator(RepositoryFactoryInterface<IAccountsRepository> contextFactory)
    {
        base.RuleFor(item => item.Email)
            .NotEmpty().WithMessage("The mail value cannot be empty")
            .EmailAddress().WithMessage("Invalid mail format")
            .Must(item =>
            {
                using var context = contextFactory.CreateRepository();
                var profile = context.AccountInfos.FirstOrDefault(p => p.Email == item);
                return profile == null;
            }).WithMessage("User is already registered");
    }
}