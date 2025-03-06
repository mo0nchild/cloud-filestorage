namespace Pinterest.Domain.Posts.Settings;

public class PostsSettings
{
    private static readonly int DefaultCleaningInterval = 10000;

    public int TagsCleaningInterval { get; set; } = DefaultCleaningInterval;
}