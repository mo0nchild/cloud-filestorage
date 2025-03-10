using AutoMapper;
using CloudStorage.Application.Tokens.Models;

namespace CloudStorage.Application.Accounts.Models;

public class IdentityModel
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
public class IdentityModelProfile : Profile
{
    public IdentityModelProfile() => CreateMap<TokensModel, IdentityModel>();
}