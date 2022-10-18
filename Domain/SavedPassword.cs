namespace Domain;

public class SavedPassword
{
    public Guid Id { get; set; }
    public string Password { get; set; } = null!;
    public string? WebAdress { get; set; }
    public string? Description { get; set; }
    public string? Login { get; set; }

    public Account Account { get; set; } = null!;
    public Guid AccountId { get; set; }
}