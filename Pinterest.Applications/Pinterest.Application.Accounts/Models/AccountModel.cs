using AutoMapper;
using Pinterest.Domain.Authorization.Entities;

namespace Pinterest.Application.Accounts.Models;

public class AccountModel
{
    public Guid Uuid { get; set; } = Guid.Empty;

    public string Email { get; set; } = string.Empty;
}

public class AccountModelProfile : Profile
{
    public AccountModelProfile() => CreateMap<AccountInfo, AccountModel>();
}