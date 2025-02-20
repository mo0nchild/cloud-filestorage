using Pinterest.Application.Users.Models;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Users.Interfaces;

public interface IUserValidators
{
    IModelValidator<NewUserInfo> NewUserValidator { get; }
    IModelValidator<UpdateUserInfo> UpdateUserValidator { get; }
}