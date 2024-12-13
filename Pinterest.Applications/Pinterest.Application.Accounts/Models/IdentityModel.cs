using AutoMapper;
using Pinterest.Application.Tokens.Models;

namespace Pinterest.Application.Accounts.Models;

public class IdentityModel
{
    public string AccessToken { get; set; } = string.Empty;
}
public class IdentityModelProfile : Profile
{
    public IdentityModelProfile() => CreateMap<TokensModel, IdentityModel>();
}