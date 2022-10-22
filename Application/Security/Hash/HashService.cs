using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace PasswordManager.Application.Security.Hash;

public class HashService : IHashService
{
    private readonly string _pepper;

    public HashService(IConfiguration configuration)
    {
        _pepper = configuration["pepper"];
    }

    public string HashWithSHA512(string text)
    {
        return CalculateSHA512(text + _pepper);
    }

    public string HashWithHMAC(string text)
    {
        return CalculateHMAC(text, _pepper);
    }

    private string CalculateSHA512(string text)
    {
        //split string into bytes
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);

        using var sha512 = SHA512.Create();
        var hashValue = sha512.ComputeHash(bytes);

        //join separate bytes into string
        return hashValue.Aggregate("", (current, hashByte) => current + $"{hashByte:x2}");
    }

    private string CalculateHMAC(string text, string key)
    {
        var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

        using var hmacSha512 = new HMACSHA512(keyBytes);
        var hashValue = hmacSha512.ComputeHash(textBytes);
        return hashValue.Aggregate("", (current, hashByte) => current + $"{hashByte:x2}");
    }
}