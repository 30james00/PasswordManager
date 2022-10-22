using System.Security.Cryptography;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public class AccountService : IAccountService
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly IHashService _hashService;
    private readonly ILogger<AccountService> _logger;

    public AccountService(DataContext dataContext, ITokenService tokenService, IHashService hashService,
        ILogger<AccountService> logger)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
        _hashService = hashService;
        _logger = logger;
    }

    public async Task<AccountDto> CreateAccount(RegisterDto registerDto)
    {
        var salt = GenerateSalt();
        var passwordHash = registerDto.IsPasswordKeptAsHash
            ? _hashService.HashWithSHA512(registerDto.Password + salt)
            : _hashService.HashWithHMAC(registerDto.Password);
        var account = new Account
        {
            Login = registerDto.Login,
            PasswordHash = passwordHash,
            IsPasswordKeptAsHash = registerDto.IsPasswordKeptAsHash,
            Salt = salt
        };

        await _dataContext.Accounts.AddAsync(account);
        return await Login(new LoginDto
        {
            Login = account.Login,
            Password = account.PasswordHash,
        });
    }

    public async Task<AccountDto> Login(LoginDto loginDto)
    {
        var account = await _dataContext.Accounts.FirstAsync(x => x.Login == loginDto.Login);
        return new AccountDto
        {
            Login = loginDto.Login,
            Token = _tokenService.CreateToken(account),
        };
    }

    public async Task ChangePassword(Guid id, string newPassword, bool isPasswordKeptAsHash)
    {
        try
        {
            //get Account
            var account = await _dataContext.Accounts.FirstAsync(x => x.Id == id);
            //generate new salt and passwordHash
            var salt = GenerateSalt();
            var passwordHash = isPasswordKeptAsHash
                ? _hashService.HashWithSHA512(newPassword + salt)
                : _hashService.HashWithHMAC(newPassword);
            //save changes
            account.PasswordHash = passwordHash;
            account.IsPasswordKeptAsHash = isPasswordKeptAsHash;
            var result = await _dataContext.SaveChangesAsync();
            if (result <= 0) _logger.LogError("Error saving new Password to Database");
        }
        catch (Exception e)
        {
            _logger.LogError("Account not found");
            Console.WriteLine(e);
        }
    }

    public async Task<bool> IfAccountExists(string login)
    {
        return await _dataContext.Accounts.AnyAsync(x => x.Login == login);
    }

    public async Task<bool> CheckPassword(Guid id, string password)
    {
        try
        {
            var account = await _dataContext.Accounts.FirstAsync(x => x.Id == id);
            var hash = account.IsPasswordKeptAsHash
                ? _hashService.HashWithSHA512(password + account.Salt)
                : _hashService.HashWithHMAC(password);
            return hash == account.PasswordHash;
        }
        catch (Exception e)
        {
            _logger.LogError("Account not found");
            return false;
        }
    }

    public async Task<bool> CheckPassword(string login, string password)
    {
        try
        {
            var account = await _dataContext.Accounts.FirstAsync(x => x.Login == login);
            var hash = account.IsPasswordKeptAsHash
                ? _hashService.HashWithSHA512(password + account.Salt)
                : _hashService.HashWithHMAC(password);
            return hash == account.PasswordHash;
        }
        catch (Exception e)
        {
            _logger.LogError("Account not found");
            return false;
        }
    }

    private string GenerateSalt()
    {
        // Generate a 128-bit salt using a sequence of
        // cryptographically strong random bytes.
        var salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
        return Convert.ToBase64String(salt);
    }
}