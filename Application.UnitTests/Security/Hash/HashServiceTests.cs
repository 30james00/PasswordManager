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

    static object[] SHA512Cases =
    {
        new object[]
        {
            "",
            "2f024addbc4a4143fe5a904172bc0b6d148f1438fd98b0c6f1527cc40e59ee4589890d5a0add6141d22dba493a00ea972266c3194014376dffbd96863db52493"
        },
        new object[]
        {
            "a",
            "9aa797317a868e9d0d321255d8448a6b554a2394bcc1b9e98666a2e5aef1c9262ee83b2c815dc055f72c289299d2016996679ec5ca5b28bc41456c787268a8b0"
        },
        new object[]
        {
            "veryLongText13123!@!3012`-",
            "e422d7db4f01b1cbc32b44eda087f0582e078d95fa443fab87ce1211ecb135d61a34e181f9fd550b47108d8b7fe455c6891c9fb2e5c190060354236ef378da4e"
        }
    };

    static object[] HMACCases =
    {
        new object[]
        {
            "pa$$w0rd", "testSalt",
            "64b10e3170df69b33e4f2a40e08b5d7a5afedfe9fc41674546d9b356553ff1f67ebcc54e366aa31da9b4964d6f03e5cbe97560ac6a48010b6545f1be5231dc5d"
        },
        new object[]
        {
            "a", "s",
            "71e876fd1029c42e36d341c81933087472b2f356c6a9221f1f09954fe8c1d62e760599cc4bebb7deaadbfa316f62718e334b6795ad5e1f84a0bac2cf15f08c78"
        },
        new object[]
        {
            "veryLongText13123!@!3012`-", "S23!#!@",
            "8060b064bbbc7189f70a588a035692a60116f844b40f895aef4eefecdf57c1b52c2fcf7934ffbcc21cad39b84c3c80800ab1c207caf6fbf223d7b5596f107694"
        }
    };

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

    [Test]
    public void HashWithHMAC_EmptyText_ThrowsArgumentException()
    {
        var hashService = new HashService(_configuration);

        Action act = () => hashService.HashWithHMAC("", "key");

        act.Should().Throw<ArgumentException>().WithMessage("Text can not be empty.");
    }    
    
    [Test]
    public void HashWithHMAC_EmptyKey_ThrowsArgumentException()
    {
        var hashService = new HashService(_configuration);

        Action act = () => hashService.HashWithHMAC("text", "");

        act.Should().Throw<ArgumentException>().WithMessage("Key can not be empty.");
    }
}