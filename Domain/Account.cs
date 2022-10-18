namespace Domain;

public class Account
{
    public Guid Id { get; set; }
    public String Login { get; set; }
    public String PasswordHash { get; set; }
    public String Salt { get; set; }
    public bool IsPasswordKeptAsHash { get; set; }
    public List<SavedPassword> SavedPasswords { get; set; } = new List<SavedPassword>();
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}