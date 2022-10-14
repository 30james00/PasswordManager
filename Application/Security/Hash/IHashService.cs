namespace PasswordManager.Application.Security.Hash;

public interface IHashService
{
    public String CalculateSHA512(String text);
    public String CalculateHMAC(String text, String key);
}