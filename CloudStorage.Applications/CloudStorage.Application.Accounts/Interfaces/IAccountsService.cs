using CloudStorage.Application.Accounts.Models;
using CloudStorage.Application.Accounts.Services;
using CloudStorage.Domain.Authorization.Entities;

namespace CloudStorage.Application.Accounts.Interfaces;

public interface IAccountsService
{
    public Task<IdentityModel> GetTokensByCredentials(CredentialsModel credentials);
    public Task<IdentityModel> GetTokensByRefreshToken(string refreshToken);
    public Task<AccountModel?> GetAccountByAccessToken(string accessToken);
    
    public Task<Guid> CreateAccount(Guid userUuid, CredentialsModel credentials);
    public Task DeleteAccount(string accessToken);
}