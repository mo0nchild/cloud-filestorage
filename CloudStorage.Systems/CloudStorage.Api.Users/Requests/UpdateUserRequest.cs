using AutoMapper;
using CloudStorage.Application.Users.Models;
using CloudStorage.Application.Users.Models.UserBasicInfo;

namespace CloudStorage.Api.Users.Requests;

public class UpdateUserRequest
{
    public required string Username { get; set; }
    public IReadOnlyList<string> UserThemes { get; set; } = new List<string>();
}

public class UpdateUserRequestProfile : Profile
{
    public UpdateUserRequestProfile()
    {
        CreateMap<UpdateUserRequest, UpdateUserInfo>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.UserThemes, opt => opt.MapFrom(src => src.UserThemes));
    }
}