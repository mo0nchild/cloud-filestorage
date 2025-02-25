using AutoMapper;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Application.Users.Models.UserBasicInfo;

public class UserInfo
{
    public required Guid Uuid { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    
    public string? ImagePath { get; set; }
    public IReadOnlyList<string> UserThemes { get; set; } = new List<string>();
}

public class UserInfoProfile : Profile
{
    public UserInfoProfile()
    {
        CreateMap<User, UserInfo>()
            .ForMember(u => u.UserThemes, o => o.MapFrom(s => s.UserThemes))
            .ForMember(u => u.Email, o => o.MapFrom(s => s.Email))
            .ForMember(u => u.Username, o => o.MapFrom(s => s.Username))
            .ForMember(u => u.ImagePath, o => o.MapFrom(s => s.PhotoPath))
            .ForMember(u => u.Uuid, o => o.MapFrom(s => s.Uuid));
        
    }
}