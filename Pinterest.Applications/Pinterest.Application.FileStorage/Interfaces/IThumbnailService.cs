using Pinterest.Application.FileStorage.Infrastructures.Models;
using Pinterest.Application.FileStorage.Models;

namespace Pinterest.Application.FileStorage.Interfaces;

public interface IThumbnailService
{
    Task ProcessFileThumbnail(Guid fileUuid);
    Task RemoveThumbnail(Guid fileUuid);
    Task<Stream> GetFileThumbnail(Guid fileUuid, FileRangeInfo? rangeInfo = default);
    Task<FileMetadata> GetThumbnailMetadata(Guid fileUuid);
}