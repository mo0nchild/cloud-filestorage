using Pinterest.Application.FileStorage.Infrastructures.Models;
using Pinterest.Application.FileStorage.Models;

namespace Pinterest.Application.FileStorage.Infrastructures.Interfaces;

public interface IFileManager
{
    Task<FileMetadata> UploadFile(Stream fileStream, StoringFileInfo fileInfo);
    Task RemoveObjectFromStorage(StoringFileInfo fileInfo);
    
    Task<string> InitiateMultipartUpload(StoringFileInfo fileInfo);
    Task<UploadDataResult> UploadPartAsync(UploadData chuckInfo);
    Task<FileMetadata?> CompleteMultipartUpload(CompleteUploadData completeInfo);
    Task RejectMultipartUpload(string uploadId, StoringFileInfo fileInfo);
    
    Task<FileMetadata> GetFileMetadata(StoringFileInfo fileInfo);
    Task<Stream> GetFileStream(StoringFileInfo fileInfo, FileRangeInfo? fileRangeInfo);
}