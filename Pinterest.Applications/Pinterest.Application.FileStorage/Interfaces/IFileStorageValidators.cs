using Pinterest.Application.FileStorage.Models;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.FileStorage.Interfaces;

public interface IFileStorageValidators
{
    IModelValidator<NewFileInfo> NewFileValidator { get; }
    IModelValidator<CompleteUploadInfo> CompleteUploadValidator { get; }
    IModelValidator<UploadChuckInfo> UploadChuckValidator { get; }
    IModelValidator<FileRangeInfo> FileRangeValidator { get; }
}