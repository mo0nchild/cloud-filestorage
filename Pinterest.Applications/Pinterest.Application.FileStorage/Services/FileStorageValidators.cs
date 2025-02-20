using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Application.FileStorage.Models;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.FileStorage.Services;

internal class FileStorageValidators : IFileStorageValidators
{
    public FileStorageValidators(IModelValidator<NewFileInfo> newFileValidator,
        IModelValidator<CompleteUploadInfo> completeUploadValidator,
        IModelValidator<FileRangeInfo> fileRangeValidator,
        IModelValidator<UploadChuckInfo> uploadChuckValidator)
    {
        NewFileValidator = newFileValidator;
        CompleteUploadValidator = completeUploadValidator;
        FileRangeValidator = fileRangeValidator;
        UploadChuckValidator = uploadChuckValidator;
    }
    public IModelValidator<NewFileInfo> NewFileValidator { get; }
    public IModelValidator<CompleteUploadInfo> CompleteUploadValidator { get; }
    public IModelValidator<FileRangeInfo> FileRangeValidator { get; }
    public IModelValidator<UploadChuckInfo> UploadChuckValidator { get; }
}