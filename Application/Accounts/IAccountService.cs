using PasswordManager.Application.Accounts.DTOs;

namespace PasswordManager.Application.Accounts;

public interface IAccountService
{
    public string GenerateSalt();
    public string GetPasswordHash(string password, string salt, bool isPasswordKeptAsHash);

    public byte[] GetMasterPasswordKey(string password);
}