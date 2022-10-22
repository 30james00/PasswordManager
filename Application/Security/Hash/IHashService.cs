namespace PasswordManager.Application.Security.Hash;

public interface IHashService
{
    public string HashWithSHA512(string text);
    public string HashWithHMAC(string text);
}