using CloudStorage.Application.Users.Models;
using CloudStorage.Application.Users.Models.UserBasicInfo;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Users.Interfaces;

public interface IUserValidators
{
    IModelValidator<NewUserInfo> NewUserValidator { get; }
    IModelValidator<UpdateUserInfo> UpdateUserValidator { get; }
}