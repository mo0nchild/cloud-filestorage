using FluentValidation;
using MongoDB.Driver;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.FileStorage.Entities;

namespace Pinterest.Application.FileStorage.Models;

public class UploadChuckInfo
{
    public required Guid FileUuid { get; set; }
    public required int PartNumber { get; set; }
    public required Stream ChuckData { get; set; }
}

public class UploadChuckInfoValidator : AbstractValidator<UploadChuckInfo>
{
    public UploadChuckInfoValidator(IDocumentRepository<StorageEntity> repository)
    {
        RuleFor(x => x.FileUuid)
            .NotEmpty().WithMessage("File uuid cannot be empty")
            .Must(x => !x.Equals(Guid.Empty)).WithMessage("File uuid must not be default");
        RuleFor(x => x.ChuckData)
            .NotEmpty().WithMessage("Chuck data cannot be empty")
            .Must(x => x.Length > 0).WithMessage("Chuck data cannot be size = 0");
        RuleFor(x => x.PartNumber)
            .Must(x => x >= 0).WithMessage("Part number cannot be negative");
    }
}