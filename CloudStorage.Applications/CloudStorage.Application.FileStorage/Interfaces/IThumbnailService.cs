using CloudStorage.Application.FileStorage.Infrastructures.Models;
using CloudStorage.Application.FileStorage.Models;

namespace CloudStorage.Application.FileStorage.Interfaces;

public interface IThumbnailService
{
    Task ProcessFileThumbnail(Guid fileUuid);
    Task RemoveThumbnail(Guid fileUuid);
    Task<Stream> GetFileThumbnail(Guid fileUuid, FileRangeInfo? rangeInfo = default);
    Task<FileMetadata> GetThumbnailMetadata(Guid fileUuid);
}