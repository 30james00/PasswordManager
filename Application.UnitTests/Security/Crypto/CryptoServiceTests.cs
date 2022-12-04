using System;
using FluentAssertions;
using NUnit.Framework;
using PasswordManager.Application.Security.Crypto;

namespace Application.UnitTests.Security.Crypto;

[TestFixture]
public class CryptoServiceTests
{
    private CryptoService _cryptoService;
    private byte[] _keyBytes = Convert.FromBase64String("Q3J93MrPXz0PEv5aQ3J93MrPXz0PEv5a");

    [OneTimeSetUp]
    public void SetUp()
    {
        _cryptoService = new CryptoService();
    }

    [TestCase("")]
    [TestCase(null)]
    public void Encrypt_EmptyString_ThrowsArgumentException(string plainText)
    {
        Action act = () => _cryptoService.Encrypt(plainText, null, out var iv);

        act.Should().Throw<ArgumentException>().WithMessage("Text can not be empty.");
    }

    [TestCase("")]
    [TestCase(null)]
    public void Decrypt_EmptyString_ThrowsArgumentException(string cipherText)
    {
        Action act = () => _cryptoService.Decrypt(cipherText, null, null);

        act.Should().Throw<ArgumentException>().WithMessage("CipherText can not be empty.");
    }

    [TestCaseSource(nameof(EncryptExamples))]
    public void Encrypt_CorrectText_EncryptedString(string plainText, int length)
    {
        var encryptedString = _cryptoService.Encrypt(plainText, _keyBytes, out var iv);

        encryptedString.Length.Should().Be(length);
        Console.WriteLine(encryptedString);
        Console.WriteLine(Convert.ToBase64String(iv));
        iv.Length.Should().Be(16);
    }

    [TestCaseSource(nameof(DecryptExamples))]
    public void Decrypt_CorrectText_DecryptedString(string cipherText, string iv, string expectedResult)
    {
        var decryptedString = _cryptoService.Decrypt(cipherText, _keyBytes, Convert.FromBase64String(iv));

        decryptedString.Should().Be(expectedResult);
    }

    private static object[] EncryptExamples =
    {
        new object[] { "a", 24 },
        new object[] { "Q3J93MrPXz0PEv5a", 44 },
        new object[] { "GVS5WtMNVmINrQEUG2/GXA47Sn/mEw+oSt9Z/Qz8vi4=", 64 },
    };

    private static object[] DecryptExamples =
    {
        new object[] { "24oFwmxLowIh6DqWdsi2LQ==", "+frEg17+40WcocpR/jyMUg==", "a" },
        new object[] { "uwdVkJZU9Z+6mFb4cIrqYWnMr8XTegL+8oAOT1nxn9I=", "vaHEUqyekWnzQfldHMQSrw==", "Q3J93MrPXz0PEv5a" },
        new object[] { "iclQhZttFUDPEPq2fzBfC2emOJMru2LPLXXuqhT9Z8JpnENgcRq/Z1bMGVcjHbLQ", "BQW9yLEQNoqUhxoUCaw9Xw==", "GVS5WtMNVmINrQEUG2/GXA47Sn/mEw+oSt9Z/Qz8vi4=" },
    };
}