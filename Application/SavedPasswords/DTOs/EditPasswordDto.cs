using System.ComponentModel.DataAnnotations;

namespace PasswordManager.Application.SavedPasswords.DTOs;

public class EditPasswordDto
{
    [Required] public Guid Id { get; set; }
    [Required] public string Password { get; set; } = null!;
    [Required] public string WebAddress { get; set; } = null!;
    public string? Description { get; set; }
    public string? Login { get; set; }
}