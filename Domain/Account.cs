namespace Domain;

public class Account
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public bool IsPasswordKeptAsHash { get; set; }
    public ICollection<SavedPassword> SavedPasswords { get; set; } = new List<SavedPassword>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();
    public ICollection<SharedPassword> SharedPasswords { get; set; } = new List<SharedPassword>();
}