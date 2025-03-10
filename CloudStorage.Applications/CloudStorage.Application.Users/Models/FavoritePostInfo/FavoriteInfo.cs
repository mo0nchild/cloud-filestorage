using AutoMapper;

namespace CloudStorage.Application.Users.Models.FavoritePostInfo;

public class FavoriteInfo
{
    public IReadOnlyList<Guid> FavoritesPosts { get; set; } = new List<Guid>();
}

public class FavoritePostProfile : Profile
{
    public FavoritePostProfile()
    {
        CreateMap<List<Domain.Users.Entities.FavoritePost>, FavoriteInfo>()
            .ForMember(dest => dest.FavoritesPosts, opt => opt.MapFrom(src => src));
    }
}