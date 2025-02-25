using AutoMapper;
using Pinterest.Application.Users.Models.UserBasicInfo;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Application.Users.Models.SubscribersInfo;

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