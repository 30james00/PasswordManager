namespace PasswordManager.Application.Security.Crypto;

public interface ICryptoService
{
    public string Encrypt(string plainText, byte[] key, out byte[] iv);
    public string Decrypt(string cipherText, byte[] key, byte[] iv);
}