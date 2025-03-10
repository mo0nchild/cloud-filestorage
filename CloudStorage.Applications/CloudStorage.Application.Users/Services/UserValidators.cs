using CloudStorage.Application.Users.Interfaces;
using CloudStorage.Application.Users.Models.UserBasicInfo;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Users.Services;

internal class UserValidators : IUserValidators
{
    public UserValidators(IModelValidator<NewUserInfo> newUserValidator, 
        IModelValidator<UpdateUserInfo> updateUserValidator)
    {
        NewUserValidator = newUserValidator;
        UpdateUserValidator = updateUserValidator;
    }
    public IModelValidator<NewUserInfo> NewUserValidator { get; }
    public IModelValidator<UpdateUserInfo> UpdateUserValidator { get; }
}