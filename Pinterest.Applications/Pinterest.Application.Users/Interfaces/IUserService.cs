using Pinterest.Application.Users.Models;
using Pinterest.Application.Users.Models.UserBasicInfo;

namespace Pinterest.Application.Users.Interfaces;

public interface IUserService
{
    Task<UserInfo> GetUserInfoAsync(Guid userUuid);
    
    Task<Guid> CreateUserAsync(NewUserInfo newUserInfo);
    Task DeleteUserAsync(Guid userUuid);
    Task UpdateUserAsync(UpdateUserInfo userInfo);
    Task UpdateUserImageAsync(UserImageInfo userImage);
}