namespace PasswordManager.Application.SharedPasswords.DTOs;

public class SharedPasswordDto
{
    public Guid Id { get; set; }
    public string WebAddress { get; set; } = null!;
    public string? Description { get; set; }
    public string? Login { get; set; }
    public string Owner { get; set; } = null!;
}