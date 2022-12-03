using NUnit.Framework;
using PasswordManager.Application.Security.Crypto;

namespace Application.UnitTests.Security.Crypto;

[TestFixture]
public class CryptoServiceTests
{
    private CryptoService _cryptoService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _cryptoService = new CryptoService();
    }

    [Test]
    public void Encrypt_CorrectString_EncryptedString()
    {
    }
}