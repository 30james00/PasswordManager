using System.Security.Cryptography;
using Domain;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public class AccountService : IAccountService
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly IHashService _hashService;
    private readonly IConfiguration _configuration;

    public AccountService(DataContext dataContext, ITokenService tokenService, IHashService hashService,
        IConfiguration configuration)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
        _hashService = hashService;
        _configuration = configuration;
    }

    public async Task<AccountDto> CreateAccount(RegisterDto registerDto)
    {
        var salt = GenerateSalt();
        var pepper = _configuration["Pepper"];
        var passwordHash = registerDto.IsPasswordKeptAsHash
            ? _hashService.CalculateSHA512(registerDto.Password + salt + pepper)
            : _hashService.CalculateHMAC(registerDto.Password, pepper);
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
        throw new NotImplementedException();
    }

    public async Task ChangePassword(ChangePasswordDto changePasswordDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IfAccountExists(string login)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CheckPassword(Guid id, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CheckPassword(string login, string password)
    {
        throw new NotImplementedException();
    }

    private string GenerateSalt()
    {
        // Generate a 128-bit salt using a sequence of
        // cryptographically strong random bytes.
        var salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
        return Convert.ToBase64String(salt);
    }
}