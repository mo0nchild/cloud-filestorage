using CloudStorage.Application.FileStorage.Interfaces;

namespace CloudStorage.Api.FileStorage.Services;

public class CleaningHostedService : BackgroundService
{
    private readonly IFileStorageService _fileStorageService;

    public CleaningHostedService(IFileStorageService fileStorageService, ILogger<CleaningHostedService> logger)
    {
        _fileStorageService = fileStorageService;
        Logger = logger;
    }
    public ILogger<CleaningHostedService> Logger { get; }

    private async Task ClearUnUsingFilesAsync() => await _fileStorageService.RemoveNotUsingFiles();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ClearUnUsingFilesAsync();
            await Task.Delay(1000, stoppingToken);
        }
    }
}