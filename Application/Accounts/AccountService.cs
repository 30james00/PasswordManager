using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using PasswordManager.Application.Security.Hash;

namespace PasswordManager.Application.Accounts;

public class AccountService : IAccountService
{
    private readonly IHashService _hashService;
    private readonly IConfiguration _configuration;


    public AccountService(IHashService hashService, IConfiguration configuration)
    {
        _hashService = hashService;
        _configuration = configuration;
    }

    public string GenerateSalt()
    {
        // Generate a 128-bit salt using a sequence of
        // cryptographically strong random bytes.
        var salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
        return Convert.ToBase64String(salt);
    }

    public string GetPasswordHash(string password, string salt, bool isPasswordKeptAsHash)
    {
        return isPasswordKeptAsHash
            ? _hashService.HashWithSHA512(password + salt)
            : _hashService.HashWithHMAC(password, salt);
    }

    public byte[] GetMasterPasswordKey(string password)
    {
        return Convert.FromBase64String(
            _hashService.HashWithMD5(_hashService.HashWithHMAC(password, _configuration["Pepper"])));
    }
}