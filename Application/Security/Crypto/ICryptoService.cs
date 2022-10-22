namespace PasswordManager.Application.Security.Crypto;

public interface ICryptoService
{
    public string Encrypt(string text, byte[] key, byte[] iv);
    public string Decrypt(string cipher, byte[] key, byte[] iv);
}