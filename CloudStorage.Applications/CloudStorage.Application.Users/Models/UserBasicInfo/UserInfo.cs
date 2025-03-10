using AutoMapper;
using CloudStorage.Domain.Users.Entities;

namespace CloudStorage.Application.Users.Models.UserBasicInfo;

public class UserInfo
{
    public required Guid Uuid { get; set; }
    public required DateTime CreatedTime { get; set; }
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
            .ForMember(dest => dest.UserThemes, opt => opt.MapFrom(src => src.UserThemes))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.PhotoPath))
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Uuid))
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));
        
    }
}