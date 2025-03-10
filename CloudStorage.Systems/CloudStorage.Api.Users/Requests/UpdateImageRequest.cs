using AutoMapper;
using CloudStorage.Application.Users.Models;
using CloudStorage.Application.Users.Models.UserBasicInfo;

namespace CloudStorage.Api.Users.Requests;

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