using Pinterest.Domain.Users.Enums;

namespace Pinterest.Application.Users.Models;

public class RegistrationModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public Gender Gender { get; set; } = default!;
    public List<string> Themes { get; set; } = new();
}