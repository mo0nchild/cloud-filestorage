using Pinterest.Application.FileStorage.Infrastructures;
using Pinterest.Application.FileStorage.Infrastructures.Models;
using Pinterest.Application.FileStorage.Models;

namespace Pinterest.Application.FileStorage.Interfaces;

public interface IFileStorageService
{
    Task<Guid> UploadFile(NewFileInfo fileInfo, Stream fileStream);
    Task<Guid> InitializeUpload(NewFileInfo fileInfo);
    Task<PartInfo> UploadFilePart(UploadChuckInfo chuckInfo);
    Task CompleteUpload(CompleteUploadInfo completeInfo);
    
    Task DeleteFile(Guid fileUuid);
    Task SetFileIsUsing(Guid fileUuid);
    
    Task<FileMetadata> GetFileMetadata(Guid fileUuid);
    Task<Stream> GetFileData(Guid fileUuid, FileRangeInfo? rangeInfo = default);
    
    Task RemoveNotUsingFiles();
}