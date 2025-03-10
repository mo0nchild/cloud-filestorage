namespace CloudStorage.Application.Users.Models.FavoritePostInfo;

public class RemoveFavoritePost
{
    public required Guid FavoriteUuid { get; set; }
    public required Guid UserUuid { get; set; }
}