namespace PasswordManager.Application.SavedPasswords.DAOs;

public class SavedPasswordDao
{
    public Guid Id { get; set; }
    public string WebAddress { get; set; } = null!;
    public string? Description { get; set; }
    public string? Login { get; set; }
    public Guid AccountId { get; set; }
}