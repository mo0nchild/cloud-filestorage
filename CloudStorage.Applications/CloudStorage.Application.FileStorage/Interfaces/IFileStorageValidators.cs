using CloudStorage.Application.FileStorage.Models;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.FileStorage.Interfaces;

public interface IFileStorageValidators
{
    IModelValidator<NewFileInfo> NewFileValidator { get; }
    IModelValidator<CompleteUploadInfo> CompleteUploadValidator { get; }
    IModelValidator<UploadChuckInfo> UploadChuckValidator { get; }
    IModelValidator<FileRangeInfo> FileRangeValidator { get; }
}