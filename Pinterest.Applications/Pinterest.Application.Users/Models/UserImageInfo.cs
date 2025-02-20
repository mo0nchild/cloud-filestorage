namespace Pinterest.Application.Users.Models;

public class UserImageInfo
{
    public required Guid UserUuid { get; set; }
    public Guid? ImageUuid { get; set; }
}