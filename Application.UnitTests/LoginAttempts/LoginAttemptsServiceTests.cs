using System;
using System.Threading.Tasks;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PasswordManager.Application.LoginAttempts;
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

        await using (var context = new DataContext(options))
        {
            context.Accounts.Add(new Account
                { Id = _accountId, Login = "test", PasswordHash = "test", Salt = "test" });
            context.SaveChanges();
        }

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            await service.LogLoginAttempt(_accountId, isSuccessful, _ipAddress);

            // Use a clean instance of the context to run the test
            var loginAttempt = await context.LoginAttempts.FirstOrDefaultAsync();
            loginAttempt?.AccountId.Should().Be(_accountId);
            loginAttempt?.Time.Should().BeWithin(5.Seconds());
            loginAttempt?.IsSuccessful.Should().Be(isSuccessful);
            loginAttempt?.IpAddress.Should().Be(_ipAddress);
        }
    }

    [Test]
    public async Task LastSuccessfulLoginAttemptTime_Exists_DataTime()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        await using (var context = new DataContext(options))
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

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            var lastSuccessfulLogin = await service.LastSuccessfulLoginAttemptTime(_accountId);

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

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            var lastSuccessfulLogin = await service.LastSuccessfulLoginAttemptTime(_accountId);

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

        await using (var context = new DataContext(options))
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

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            var lastUnsuccessfulLoginAttemptTime = await service.LastUnsuccessfulLoginAttemptTime(_accountId);

            // Use a clean instance of the context to run the test
            lastUnsuccessfulLoginAttemptTime.Should().BeWithin(3.Seconds());
        }
    }

    [Test]
    public async Task LastUnsuccessfulLoginAttemptTime_NotExist_Null()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            var lastUnsuccessfulLogin = await service.LastSuccessfulLoginAttemptTime(_accountId);

            // Use a clean instance of the context to run the test
            lastUnsuccessfulLogin.Should().BeNull();
        }
    }

    [TestCase(1, 0)]
    [TestCase(2, 5)]
    [TestCase(3, 10)]
    [TestCase(4, 120)]
    [TestCase(5, 120)]
    public async Task ThrottleLogIn_UnsuccessfulLoginAttempts_XSecLockout(int attemptsNumber, int lockoutTime)
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        await using (var context = new DataContext(options))
        {
            for (int i = 0; i < attemptsNumber; i++)
            {
                await context.LoginAttempts.AddAsync(new LoginAttempt
                {
                    AccountId = _accountId,
                    IpAddress = _ipAddress,
                    IsSuccessful = false,
                    Time = DateTime.Now.Subtract(TimeSpan.FromDays(1))
                });
            }

            await context.SaveChangesAsync();
        }

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            var time = await service.ThrottleLogInTime(_accountId);

            time.Should().Be(lockoutTime);
        }
    }

    [TestCase(1, 0)]
    [TestCase(10, 50)]
    [TestCase(15, 100)]
    [TestCase(30, int.MaxValue)]
    [TestCase(50, int.MaxValue)]
    public async Task ThrottleIpLogIn_NotBlockedIp_XSecLockout(int attemptsNumber, int lockoutTime)
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        await using (var context = new DataContext(options))
        {
            for (int i = 0; i < attemptsNumber; i++)
            {
                await context.LoginAttempts.AddAsync(new LoginAttempt
                {
                    AccountId = _accountId,
                    IpAddress = _ipAddress,
                    IsSuccessful = false,
                    Time = DateTime.Now.Subtract(TimeSpan.FromDays(1))
                });
            }

            await context.SaveChangesAsync();
        }

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            var time = await service.ThrottleIpLogIn(_accountId, _ipAddress);

            time.Should().Be(lockoutTime);
        }
    }

    [Test]
    public async Task ThrottleIpLogInBlockedIp_PermanentBlock_MaxInt()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        await using (var context = new DataContext(options))
        {
            await context.IpAddressBlocks.AddAsync(new IpAddressBlock
            {
                AccountId = _accountId,
                IpAddress = _ipAddress,
            });
            await context.SaveChangesAsync();
        }

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            var time = await service.ThrottleIpLogIn(_accountId, _ipAddress);

            time.Should().Be(int.MaxValue);
        }
    }

    [Test]
    public async Task ThrottleIpLogInBlockedIp_4UnsuccessfulAttempts_BlockIP()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(
                $"{Guid.NewGuid()}")
            .Options;

        await using (var context = new DataContext(options))
        {
            await context.IpAddressBlocks.AddAsync(new IpAddressBlock
            {
                AccountId = _accountId,
                IpAddress = _ipAddress,
            });
            await context.SaveChangesAsync();
        }

        await using (var context = new DataContext(options))
        {
            var service = new LoginAttemptsService(context);

            var time = await service.ThrottleIpLogIn(_accountId, _ipAddress);
            var block = await context.IpAddressBlocks.FirstOrDefaultAsync(x =>
                x.IpAddress == _ipAddress && x.AccountId == _accountId);

            time.Should().Be(int.MaxValue);
            block?.IpAddress.Should().Be(_ipAddress);
            block?.AccountId.Should().Be(_accountId);
        }
    }
}