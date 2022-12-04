using System;
using System.Collections.Generic;
using Domain;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PasswordManager.Application.Security.Token;

namespace Application.UnitTests.Security.Token;

public class TokenServiceTests
{
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "TokenKey", "testTokenKeyVerySecure" },
        })
        .Build();

    [Test]
    public void CreateToken_Account_JWTToken()
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Login = "test",
            Salt = "test",
            PasswordHash = "test",
            IsPasswordKeptAsHash = false,
        };
        var tokenService = new TokenService(_configuration);

        var token = tokenService.CreateToken(account);

        token.Should().NotBeEmpty();
    }

    [Test]
    public void GenerateRefreshToken_Random_RefreshToken()
    {
        var tokenService = new TokenService(_configuration);

        var refreshToken = tokenService.GenerateRefreshToken();

        refreshToken.Should().BeOfType<RefreshToken>();
        refreshToken.Token.Should().NotBeEmpty();
    }
}