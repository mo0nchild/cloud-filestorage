using FluentValidation;

namespace CloudStorage.Application.FileStorage.Models;

public class CompleteUploadInfo
{
    public required Guid FileUuid { get; set; }
    public required IReadOnlyList<PartInfo> Parts { get; set; }
}
public class PartInfo
{
    public required string ETag { get; set; }
    public required int PartNumber { get; set; }
}

public class CompleteUploadInfoValidator : AbstractValidator<CompleteUploadInfo>
{
    public CompleteUploadInfoValidator()
    {
        RuleFor(x => x.FileUuid)
            .NotEmpty().WithMessage("File uuid cannot be empty")
            .Must(x => !x.Equals(Guid.Empty)).WithMessage("File uuid must not be default");
        RuleFor(x => x.Parts)
            .NotEmpty().WithMessage("Parts list cannot be empty");
    }
}