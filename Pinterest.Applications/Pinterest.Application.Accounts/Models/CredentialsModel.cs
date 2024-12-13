using AutoMapper;
using Pinterest.Domain.Authorization.Entities;

namespace Pinterest.Application.Accounts.Models;

public class CredentialsModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CredentialsModelProfile : Profile
{
    public CredentialsModelProfile() : base() => base.CreateMap<CredentialsModel, AccountInfo>();
}