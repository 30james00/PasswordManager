using System;
using System.Threading.Tasks;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PasswordManager.Application.LoginAttempts;
using PasswordManager.Application.Security.Token;
using DataContext = Infrastructure.DataContext;

namespace Application.UnitTests.LoginAttempts;

public class LoginAttemptsServiceTests
{
    private Guid _accountId = Guid.Parse("90b24f0e-7e4c-11ed-a1eb-0242ac120002");
    private string _ipAddress = "127.0.0.1";


    // [SetUp]
    // public void SetUp()
    // {
    //     // Insert seed data into the database using one instance of the context
    //     using (var context = new DataContext(_options))
    //     {
    //         context.Accounts.Add(new Account
    //             { Id = _accountId, Login = "test", PasswordHash = "test", Salt = "test" });
    //         context.SaveChanges();
    //     }
    // }

    [TestCase(true)]
    [TestCase(false)]
    public async Task LogLoginAttempt_SuccessfulAndUnsuccessful_Log(bool isSuccessful)
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        using (var context = new DataContext(options))
        {
            context.Accounts.Add(new Account
                { Id = _accountId, Login = "test", PasswordHash = "test", Salt = "test" });
            context.SaveChanges();
        }

        using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context, _accountId);

            service.LogLoginAttempt(isSuccessful, _ipAddress);

            // Use a clean instance of the context to run the test
            var loginAttempt = await context.LoginAttempts.FirstOrDefaultAsync();
            loginAttempt.AccountId.Should().Be(_accountId);
            loginAttempt.Time.Should().BeWithin(5.Seconds());
            loginAttempt.IsSuccessful.Should().Be(isSuccessful);
            loginAttempt.IpAddress.Should().Be(_ipAddress);
        }
    }

    [Test]
    public async Task LastSuccessfulLoginAttemptTime_Exists_DataTime()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        using (var context = new DataContext(options))
        {
            await context.LoginAttempts.AddAsync(new LoginAttempt
            {
                AccountId = _accountId,
                IpAddress = _ipAddress,
                IsSuccessful = true,
                Time = DateTime.Now.Subtract(TimeSpan.FromDays(1))
            });
            await context.LoginAttempts.AddAsync(new LoginAttempt
            {
                AccountId = _accountId,
                IpAddress = _ipAddress,
                IsSuccessful = true,
                Time = DateTime.Now,
            });
            await context.SaveChangesAsync();
        }

        using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context, _accountId);

            var lastSuccessfulLogin = service.LastSuccessfulLoginAttemptTime();

            // Use a clean instance of the context to run the test
            lastSuccessfulLogin.Should().BeWithin(3.Seconds());
        }
    }

    [Test]
    public async Task LastSuccessfulLoginAttemptTime_NotExist_Null()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context, _accountId);

            var lastSuccessfulLogin = service.LastSuccessfulLoginAttemptTime();

            // Use a clean instance of the context to run the test
            lastSuccessfulLogin.Should().BeNull();
        }
    } 
    
    [Test]
    public async Task LastUnsuccessfulLoginAttemptTime_Exists_DataTime()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        using (var context = new DataContext(options))
        {
            await context.LoginAttempts.AddAsync(new LoginAttempt
            {
                AccountId = _accountId,
                IpAddress = _ipAddress,
                IsSuccessful = false,
                Time = DateTime.Now.Subtract(TimeSpan.FromDays(1))
            });
            await context.LoginAttempts.AddAsync(new LoginAttempt
            {
                AccountId = _accountId,
                IpAddress = _ipAddress,
                IsSuccessful = false,
                Time = DateTime.Now,
            });
            await context.SaveChangesAsync();
        }

        using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context, _accountId);

            var lastSuccessfulLogin = service.LastUnsuccessfulLoginAttemptTime();

            // Use a clean instance of the context to run the test
            lastSuccessfulLogin.Should().BeWithin(3.Seconds());
        }
    }

    [Test]
    public async Task LastUnsuccessfulLoginAttemptTime_NotExist_Null()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context, _accountId);

            var lastSuccessfulLogin = service.LastSuccessfulLoginAttemptTime();

            // Use a clean instance of the context to run the test
            lastSuccessfulLogin.Should().BeNull();
        }
    }
}