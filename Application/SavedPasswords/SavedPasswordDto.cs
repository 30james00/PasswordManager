namespace PasswordManager.Application.DTOs;

public class SavedPasswordDto
{
    public Guid Id { get; set; }
    public string Password { get; set; } = null!;
    public string? WebAddress { get; set; }
    public string? Description { get; set; }
    public string? Login { get; set; }
    public Guid AccountId { get; set; }
}