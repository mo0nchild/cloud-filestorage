using System.Security.Claims;
using CloudStorage.Application.Tokens.Models;

namespace CloudStorage.Application.Tokens.Interfaces;

public interface ITokenService
{
    public Task<TokensModel> CreateJwtTokens(Claim[] claims);
    public Task<Claim[]?> VerifyAccessToken(string accessToken);
    public Task<Claim[]?> VerifyRefreshToken(string refreshToken);
}