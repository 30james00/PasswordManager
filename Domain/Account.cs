namespace Domain;

public class Account
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public bool IsPasswordKeptAsHash { get; set; }
    public List<SavedPassword> SavedPasswords { get; set; } = new List<SavedPassword>();
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public List<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();
}