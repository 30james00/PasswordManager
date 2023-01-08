namespace PasswordManager.Application.IpAddressBlocks.DTOs;

public class IpAddressBlockDto
{
    public Guid id { get; set; }
    public Guid AccountId { get; set; }
    public string IpAddress { get; set; } = null!;
}