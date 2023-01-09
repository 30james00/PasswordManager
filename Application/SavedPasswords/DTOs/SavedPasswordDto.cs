using PasswordManager.Application.SharedPasswords.DTOs;

namespace PasswordManager.Application.SavedPasswords.DTOs;

public class SavedPasswordDto
{
    public Guid Id { get; set; }
    public string WebAddress { get; set; } = null!;
    public string? Description { get; set; }
    public string? Login { get; set; }
    public List<SharedPasswordMiniDto> SharedTo { get; set; }
}