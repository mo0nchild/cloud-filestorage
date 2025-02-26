using Microsoft.EntityFrameworkCore;
using Pinterest.Domain.Authorization.Entities;
using Pinterest.Domain.Core.Repositories;

namespace Pinterest.Application.Accounts.Repositories;

public interface IAccountsRepository : IBaseRepository
{
    DbSet<AccountInfo> AccountInfos { get; set; }
}