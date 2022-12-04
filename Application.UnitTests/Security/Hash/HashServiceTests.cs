using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PasswordManager.Application.Security.Hash;

namespace Application.UnitTests.Security.Hash;

public class HashServiceTests
{
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "Pepper", "testPepper" },
        })
        .Build();

    [TestCaseSource(nameof(SHA512Cases))]
    public void HashWithSHA512_String_ReturnsHash(string text, string expText)
    {
        // Arrange
        var hashService = new HashService(_configuration);
        // Act
        var hash = hashService.HashWithSHA512(text);
        // Assert
        hash.Should().Be(expText);
    }

    [TestCaseSource(nameof(HMACCases))]
    public void HashWithHMAC_CorrectTextAndKey_ReturnsHash(string text, string key, string expText)
    {
        var hashService = new HashService(_configuration);

        var hash = hashService.HashWithHMAC(text, key);

        hash.Should().Be(expText);
    }    
    
    [TestCaseSource(nameof(MD5Cases))]
    public void HashWithMD5_CorrectTextAndKey_ReturnsHash(string text, string expText)
    {
        var hashService = new HashService(_configuration);

        var hash = hashService.HashWithMD5(text);

        hash.Should().Be(expText);
    }

    [TestCase("")]
    [TestCase(null)]
    public void HashWithHMAC_EmptyText_ThrowsArgumentException(string text)
    {
        var hashService = new HashService(_configuration);

        Action act = () => hashService.HashWithHMAC(text, "key");

        act.Should().Throw<ArgumentException>().WithMessage("Text can not be empty.");
    }

    [TestCase("")]
    [TestCase(null)]
    public void HashWithHMAC_EmptyKey_ThrowsArgumentException(string key)
    {
        var hashService = new HashService(_configuration);

        Action act = () => hashService.HashWithHMAC("text", key);

        act.Should().Throw<ArgumentException>().WithMessage("Key can not be empty.");
    }    
    
    [TestCase("")]
    [TestCase(null)]
    public void HashWithMD5_EmptyText_ThrowsArgumentException(string text)
    {
        var hashService = new HashService(_configuration);

        Action act = () => hashService.HashWithMD5(text);

        act.Should().Throw<ArgumentException>().WithMessage("Text can not be empty.");
    }

    static object[] SHA512Cases =
    {
        new object[]
        {
            "",
            "LwJK3bxKQUP+WpBBcrwLbRSPFDj9mLDG8VJ8xA5Z7kWJiQ1aCt1hQdItukk6AOqXImbDGUAUN23/vZaGPbUkkw=="
        },
        new object[]
        {
            "a",
            "mqeXMXqGjp0NMhJV2ESKa1VKI5S8wbnphmai5a7xySYu6DssgV3AVfcsKJKZ0gFplmeexcpbKLxBRWx4cmiosA=="
        },
        new object[]
        {
            "veryLongText13123!@!3012`-",
            "5CLX208BscvDK0TtoIfwWC4HjZX6RD+rh84SEeyxNdYaNOGB+f1VC0cQjYt/5FXGiRyfsuXBkAYDVCNu83jaTg=="
        }
    };

    static object[] HMACCases =
    {
        new object[]
        {
            "pa$$w0rd", "testSalt",
            "ZLEOMXDfabM+TypA4Itdelr+3+n8QWdFRtmzVlU/8fZ+vMVONmqjHam0lk1vA+XL6XVgrGpIAQtlRfG+UjHcXQ=="
        },
        new object[]
        {
            "a", "s",
            "ceh2/RApxC4200HIGTMIdHKy81bGqSIfHwmVT+jB1i52BZnMS+u33qrb+jFvYnGOM0tnla1eH4SgusLPFfCMeA=="
        },
        new object[]
        {
            "veryLongText13123!@!3012`-", "S23!#!@",
            "gGCwZLu8cYn3CliKA1aSpgEW+ES0D4la707v7N9XwbUsL895NP+8whytObhMPICACrHCB8r2+/Ij17VZbxB2lA=="
        }
    };    
    
    static object[] MD5Cases =
    {
        new object[]
        {
            "a",
            "DMF1ucDxtqgxw5niaXcmYQ=="
        },
        new object[]
        {
            "sbinnala",
            "wVKdMtJKyX0MDEgLxmKaxA=="
        },
        new object[]
        {
            "veryLongText13123!@!3012`-",
            "Q1jMSbiiaYfsUqDSNnw5OA=="
        }
    };
}