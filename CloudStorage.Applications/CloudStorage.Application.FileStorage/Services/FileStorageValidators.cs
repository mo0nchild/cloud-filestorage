using CloudStorage.Application.FileStorage.Interfaces;
using CloudStorage.Application.FileStorage.Models;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.FileStorage.Services;

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