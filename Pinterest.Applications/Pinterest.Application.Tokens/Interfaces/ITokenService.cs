using System.Security.Claims;
using Pinterest.Application.Tokens.Models;

namespace Pinterest.Application.Tokens.Interfaces;

public interface ITokenService
{
    public Task<TokensModel> CreateJwtTokens(Claim[] claims);
    public Task<Claim[]?> VerifyAccessToken(string token);
    public Task<Claim[]?> VerifyRefreshToken(string token);
}