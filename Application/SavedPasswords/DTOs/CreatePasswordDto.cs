using System.ComponentModel.DataAnnotations;

namespace PasswordManager.Application.SavedPasswords.DTOs;

public class CreatePasswordDto
{
    [Required] public string Password { get; set; } = null!;
    [Required] public string MasterPassword { get; set; } = null!;
    [Required] public string WebAddress { get; set; } = null!;
    public string? Description { get; set; }
    public string? Login { get; set; }
}