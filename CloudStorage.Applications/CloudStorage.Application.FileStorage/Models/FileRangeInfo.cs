using FluentValidation;

namespace CloudStorage.Application.FileStorage.Models;

public class FileRangeInfo
{
    public required long Start { get; set; }
    public required long End { get; set; }
}

public class FileRangeInfoValidator : AbstractValidator<FileRangeInfo>
{
    public FileRangeInfoValidator()
    {
        RuleFor(x => x.Start)
            .GreaterThanOrEqualTo(0).WithMessage("Start must be greater than or equal to 0");
        RuleFor(x => x.End)
            .GreaterThan(0).WithMessage("End must be greater than 0")
            .Must((item, x) => x > item.Start).WithMessage("End must be greater than start");

    }
}