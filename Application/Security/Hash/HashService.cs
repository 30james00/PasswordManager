using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PasswordManager.Application.Security.Hash;

public class HashService : IHashService
{
    private readonly string _pepper;

    public HashService(IConfiguration configuration)
    {
        _pepper = configuration["Pepper"];
    }

    public string HashWithSHA512(string text)
    {
        return CalculateSHA512(text + _pepper);
    }

    public string HashWithHMAC(string text, string key)
    {
        return CalculateHMAC(text, key);
    }

    public string HashWithMD5(string text)
    {
        // Convert string to byte array
        byte[] bytes = Encoding.UTF8.GetBytes(text);

        // Get instance of MD5 hashing algorithm
        using var md5 = MD5.Create();
        byte[] hashValue = md5.ComputeHash(bytes);

        return Convert.ToBase64String(hashValue);
    }

    private string CalculateSHA512(string text)
    {
        // Convert string to byte array
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);

        // Get instance of SHA-512 hashing algorithm
        using var sha512 = SHA512.Create();
        var hashValue = sha512.ComputeHash(bytes);

        //join separate bytes into string
        return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
    }

    private string CalculateHMAC(string text, string key)
    {
        //Check if text or key is empty 
        if (text.Length == 0) throw new ArgumentException("Text can not be empty.");
        if (key.Length == 0) throw new ArgumentException("Key can not be empty.");

        // Convert strings to byte array
        var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

        // Get instance of HMAC algorithm
        using var hmacSha512 = new HMACSHA512(keyBytes);
        var hashValue = hmacSha512.ComputeHash(textBytes);
        return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
    }
}