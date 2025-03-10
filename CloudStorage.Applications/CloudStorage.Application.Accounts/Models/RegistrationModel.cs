using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CloudStorage.Application.Accounts.Repositories;
using CloudStorage.Application.Accounts.Sagas;
using CloudStorage.Domain.Authorization.Entities;
using CloudStorage.Domain.Core.Factories;

namespace CloudStorage.Application.Accounts.Models;

public class RegistrationModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class RegistrationModelProfile : Profile
{
    public RegistrationModelProfile()
    {
        CreateMap<RegistrationModel, AccountSagaState>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
    }
}

public class RegistrationModelValidator : AbstractValidator<RegistrationModel>
{
    public RegistrationModelValidator()
    {
        RuleFor(item => item.Email)
            .NotEmpty().WithMessage("The mail value cannot be empty")
            .EmailAddress().WithMessage("Invalid mail format");
        RuleFor(item => item.Password)
            .NotEmpty().WithMessage("The password value cannot be empty");
        RuleFor(item => item.Username)
            .NotEmpty().WithMessage("The username value cannot be empty");
    }
}