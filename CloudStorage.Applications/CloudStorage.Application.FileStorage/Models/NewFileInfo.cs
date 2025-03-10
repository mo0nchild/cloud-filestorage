using AutoMapper;
using FluentValidation;
using CloudStorage.Application.FileStorage.Infrastructures;
using CloudStorage.Application.FileStorage.Infrastructures.Models;

namespace CloudStorage.Application.FileStorage.Models;

public class NewFileInfo
{
    public required string FileName { get; set; }
}

public class NewFileInfoProfile : Profile
{
    public NewFileInfoProfile()
    {
        CreateMap<NewFileInfo, StoringFileInfo>()
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName));
    }
}

public class NewFileInfoValidator : AbstractValidator<NewFileInfo>
{
    public NewFileInfoValidator()
    {
        RuleFor(src => src.FileName)
            .NotEmpty().WithMessage("File name cannot be empty")
            .NotNull().WithMessage("File name cannot be empty")
            .Must(BeAValidFileName).WithMessage("File name must be a valid")
            .Must(HaveValidExtension).WithMessage("File name must be a valid file extension");
    }
    private bool BeAValidFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        var name = Path.GetFileName(fileName);
        var invalidChars = Path.GetInvalidFileNameChars();
        return !name.Any(c => invalidChars.Contains(c));
    }
    private bool HaveValidExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        var name = Path.GetFileName(fileName);
        var lastDotIndex = name.LastIndexOf('.');
        return lastDotIndex > 0 && lastDotIndex < name.Length - 1;
    }
}