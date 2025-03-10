using Microsoft.EntityFrameworkCore;
using CloudStorage.Domain.Authorization.Entities;
using CloudStorage.Domain.Core.Repositories;

namespace CloudStorage.Application.Accounts.Repositories;

public interface IAccountsRepository : IBaseRepository
{
    DbSet<AccountInfo> AccountInfos { get; set; }
}