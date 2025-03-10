using AutoMapper;
using CloudStorage.Application.Users.Models.UserBasicInfo;
using CloudStorage.Domain.Users.Entities;

namespace CloudStorage.Application.Users.Models.SubscribersInfo;

public class SubscriptionInfo
{
    public IReadOnlyList<UserInfo> Users { get; set; } = new List<UserInfo>();
}

public class SubscriptionInfoProfile : Profile
{
    public SubscriptionInfoProfile()
    {
        CreateMap<List<User>, SubscriptionInfo>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src));
    }
}