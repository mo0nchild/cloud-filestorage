﻿using AutoMapper;
using CloudStorage.Domain.Authorization.Entities;

namespace CloudStorage.Application.Accounts.Models;

public class AccountModel
{
    public required Guid Uuid { get; set; }
    public required Guid UserUuid { get; set; }
    public required string Email { get; set; }
    public required AccountRole Role { get; set; } 
}

public class AccountModelProfile : Profile
{
    public AccountModelProfile() => CreateMap<AccountInfo, AccountModel>();
}