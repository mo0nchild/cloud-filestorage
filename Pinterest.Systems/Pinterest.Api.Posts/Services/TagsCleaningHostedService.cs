using Microsoft.Extensions.Options;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Domain.Posts.Settings;

namespace Pinterest.Api.Posts.Services;

public class TagsCleaningHostedService: BackgroundService
{
    private readonly ITagsService _tagsService;

    public TagsCleaningHostedService(ITagsService tagsService, IOptions<PostsSettings> postsSettings,
        ILogger<TagsCleaningHostedService> logger)
    {
        _tagsService = tagsService;
        PostsSettings = postsSettings.Value;
        Logger = logger;
    }
    private PostsSettings PostsSettings { get; }
    private ILogger<TagsCleaningHostedService> Logger { get; }

    private async Task ClearUnusedTagsAsync()
    {
        var jobs = await _tagsService.RemoveUnusedTags();
        if (jobs != null) await Task.WhenAll(jobs);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ClearUnusedTagsAsync();
            await Task.Delay(PostsSettings.TagsCleaningInterval, stoppingToken);
        }
    }
}