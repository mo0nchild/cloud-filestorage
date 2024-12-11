using Pinterest.Application.Users.Models;

namespace Pinterest.Application.Users.Interfaces;

public interface IUserService
{
    public Task Registrate(RegistrationModel info);
    public Task<UserInfo> GetUserInfo(Guid userUuid);
}