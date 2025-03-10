namespace CloudStorage.Application.Users.Models.UserBasicInfo;

public class UserImageInfo
{
    public required Guid UserUuid { get; set; }
    public Guid? ImageUuid { get; set; }
}