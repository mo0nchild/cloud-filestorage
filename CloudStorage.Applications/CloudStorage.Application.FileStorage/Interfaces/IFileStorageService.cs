using CloudStorage.Application.FileStorage.Infrastructures;
using CloudStorage.Application.FileStorage.Infrastructures.Models;
using CloudStorage.Application.FileStorage.Models;

namespace CloudStorage.Application.FileStorage.Interfaces;

public interface IFileStorageService
{
    Task<Guid> UploadFile(NewFileInfo fileInfo, Stream fileStream);
    Task<Guid> InitializeUpload(NewFileInfo fileInfo);
    Task<PartInfo> UploadFilePart(UploadChuckInfo chuckInfo);
    Task CompleteUpload(CompleteUploadInfo completeInfo);
    
    Task DeleteFile(Guid fileUuid);
    Task SetFileIsUsing(Guid fileUuid);
    
    Task<FileBasicInfo> GetFileMetadata(Guid fileUuid);
    Task<Stream> GetFileData(Guid fileUuid, FileRangeInfo? rangeInfo = default);
    
    Task RemoveNotUsingFiles();
}