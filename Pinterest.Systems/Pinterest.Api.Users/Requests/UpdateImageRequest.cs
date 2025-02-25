using AutoMapper;
using Pinterest.Application.Users.Models;
using Pinterest.Application.Users.Models.UserBasicInfo;

namespace Pinterest.Api.Users.Requests;

public class UpdateImageRequest
{
    public Guid? ImageUuid { get; set; }
}

public class UpdateImageRequestProfile : Profile
{
    public UpdateImageRequestProfile()
    {
        CreateMap<UpdateImageRequest, UserImageInfo>()
            .ForMember(dest => dest.ImageUuid, opt => opt.MapFrom(from => from.ImageUuid));
    }
}