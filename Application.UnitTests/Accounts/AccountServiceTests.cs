using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Security.Hash;

namespace Application.UnitTests.Accounts;

public class AccountServiceTests
{
    private readonly Mock<IHashService> _mockHashService = new Mock<IHashService>();

    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "Pepper", "testPepper" },
        })
        .Build();

    public AccountServiceTests()
    {
        _mockHashService.Setup(service => service.HashWithSHA512(It.IsAny<string>())).Returns("SHA5");
        _mockHashService.Setup(service => service.HashWithHMAC(It.IsAny<string>(), It.IsAny<string>())).Returns("HMAC");
        _mockHashService.Setup(service => service.HashWithMD5(It.IsAny<string>())).Returns("MD55");
    }

    [Test]
    public void GenerateSalt_Random_Salt()
    {
        var accountService = new AccountService(_mockHashService.Object, _configuration);

        var salt = accountService.GenerateSalt();

        salt.Length.Should().Be(24);
    }

    [Test]
    public void GetPasswordHash_NotAsHash_HMAC()
    {
        var accountService = new AccountService(_mockHashService.Object, _configuration);

        var hmac = accountService.GetPasswordHash("test", "test", false);

        hmac.Should().Be("HMAC");
    }

    [Test]
    public void GetPasswordHash_AsHash_SHA512()
    {
        var accountService = new AccountService(_mockHashService.Object, _configuration);

        var sha = accountService.GetPasswordHash("test", "test", true);

        sha.Should().Be("SHA5");
    }

    [Test]
    public void GetMasterPasswordKey_Password_MD5()
    {
        var accountService = new AccountService(_mockHashService.Object, _configuration);

        var md5 = accountService.GetMasterPasswordKey("test");

        md5.Length.Should().Be(3);
    }
}