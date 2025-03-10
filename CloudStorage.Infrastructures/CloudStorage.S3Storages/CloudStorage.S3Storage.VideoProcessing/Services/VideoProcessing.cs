using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CloudStorage.Application.FileStorage.Infrastructures.Interfaces;
using CloudStorage.S3Storage.VideoProcessing.Settings;
using Xabe.FFmpeg;

namespace CloudStorage.S3Storage.VideoProcessing.Services;

internal class VideoProcessing : IVideoProcessing
{
    private readonly string _tempDirectory;
    
    public VideoProcessing(IOptions<ProcessingSettings> options, ILogger<VideoProcessing> logger)
    {
        (Options, Logger) = (options.Value, logger);
        _tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Options.TempDirectory);
        
        if (!Directory.Exists(_tempDirectory)) Directory.CreateDirectory(_tempDirectory);
    }
    private ILogger<VideoProcessing> Logger { get; }
    private ProcessingSettings Options { get; }
    
    public async Task<Stream> GetVideoThumbnail(string filePath)
    {
        var thumbTempPath = Path.Combine(_tempDirectory, $"{Guid.NewGuid()}.jpg");
        var videoMemoryStream = new MemoryStream();
        try {
            var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(filePath, thumbTempPath, TimeSpan.FromSeconds(0));
            conversion.AddParameter("-q:v 5");
            conversion.AddParameter("-vf scale=iw/2:ih/2");
            await conversion.Start();

            await using (var fileStream = new FileStream(thumbTempPath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(videoMemoryStream);
            }
        }
        finally { File.Delete(thumbTempPath); }
        return videoMemoryStream;
    }
}