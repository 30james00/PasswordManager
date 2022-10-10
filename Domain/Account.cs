namespace Domain;

public class Account
{
    public Guid Id { get; set; }
    public String Login { get; set; }
    public String PasswordHash { get; set; }
    public String Salt { get; set; }
    public bool IsPasswordKeptAsHash { get; set; }
}