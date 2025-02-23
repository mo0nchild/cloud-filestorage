namespace Pinterest.Application.FileStorage.Infrastructures.Interfaces;

public interface IVideoProcessing
{
    Task<Stream> GetVideoThumbnail(string filePath);
}