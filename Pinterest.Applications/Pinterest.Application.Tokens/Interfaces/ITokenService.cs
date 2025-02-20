using System.Security.Claims;
using Pinterest.Application.Tokens.Models;

namespace Pinterest.Application.Tokens.Interfaces;

public interface ITokenService
{
    public Task<TokensModel> CreateJwtTokens(Claim[] claims);
    public Task<Claim[]?> VerifyAccessToken(string accessToken);
    public Task<Claim[]?> VerifyRefreshToken(string refreshToken);
}