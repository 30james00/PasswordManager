namespace PasswordManager.Application.LoginAttempts.DTOs;

public class IpAddressBlockDto
{
    public Guid AccountId { get; set; }
    public string IpAddress { get; set; } = null!;
}