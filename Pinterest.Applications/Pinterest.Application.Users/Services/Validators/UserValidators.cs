using Pinterest.Application.Users.Interfaces;
using Pinterest.Application.Users.Models;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Users.Services.Validators;

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