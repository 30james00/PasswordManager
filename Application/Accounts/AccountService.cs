using System.Security.Cryptography;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.SavedPasswords;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public class AccountService : IAccountService
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly IHashService _hashService;
    private readonly IConfiguration _configuration;
    private readonly ICryptoService _cryptoService;
    private readonly ISavedPasswordService _savedPasswordService;

    public AccountService(DataContext dataContext, ITokenService tokenService, IHashService hashService,
        IConfiguration configuration, ICryptoService cryptoService, ISavedPasswordService savedPasswordService)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
        _hashService = hashService;
        _configuration = configuration;
        _cryptoService = cryptoService;
        _savedPasswordService = savedPasswordService;
    }

    public async Task<AccountDto> CreateAccount(RegisterDto registerDto)
    {
        var salt = GenerateSalt();
        var passwordHash = GetPasswordHash(registerDto.Password, salt, registerDto.IsPasswordKeptAsHash);
        var account = new Account
        {
            Login = registerDto.Login,
            PasswordHash = passwordHash,
            IsPasswordKeptAsHash = registerDto.IsPasswordKeptAsHash,
            Salt = salt,
        };

        await _dataContext.Accounts.AddAsync(account);
        var result = await _dataContext.SaveChangesAsync();
        if (result <= 0) throw new Exception("Error saving new Account to Database");
        return new AccountDto
        {
            Login = account.Login,
            Token = _tokenService.CreateToken(account),
        };
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

    public async Task<AccountDto> ChangePassword(Guid id, string newPassword, bool isPasswordKeptAsHash)
    {
        //get Account
        var account = await _dataContext.Accounts.FirstAsync(x => x.Id == id);
        //generate new salt and passwordHash
        var salt = GenerateSalt();
        var passwordHash = GetPasswordHash(newPassword, salt, isPasswordKeptAsHash);

        //decrypt passwords
        var savedPasswords = await _dataContext.SavedPasswords.Where(x => x.AccountId == account.Id).ToListAsync();
        foreach (var savedPassword in savedPasswords)
        {
            savedPassword.Password = await _savedPasswordService.DecryptPassword(savedPassword.Id);
        }

        //save changes
        account.PasswordHash = passwordHash;
        account.Salt = salt;
        account.IsPasswordKeptAsHash = isPasswordKeptAsHash;

        foreach (var savedPassword in savedPasswords)
        {
            var masterPasswordBytes = GetMasterPasswordBytes(passwordHash);
            using var aes = Aes.Create();
            var ivBytes = aes.IV;
            savedPassword.Password = _cryptoService.Encrypt(savedPassword.Password, masterPasswordBytes, ivBytes);
            savedPassword.Iv = Convert.ToBase64String(ivBytes);
        }

        var result = await _dataContext.SaveChangesAsync();
        if (result <= 0) throw new Exception("Error saving new MasterPassword to Database");
        return new AccountDto
        {
            Login = account.Login,
            Token = _tokenService.CreateToken(account),
        };
    }

    public async Task<bool> IfAccountExists(string login)
    {
        return await _dataContext.Accounts.AnyAsync(x => x.Login == login);
    }

    public async Task<bool> CheckPassword(Guid id, string password)
    {
        var account = await _dataContext.Accounts.FirstAsync(x => x.Id == id);
        var hash = GetPasswordHash(password, account.Salt, account.IsPasswordKeptAsHash);
        return hash == account.PasswordHash;
    }

    public async Task<bool> CheckPassword(string login, string password)
    {
        var account = await _dataContext.Accounts.FirstAsync(x => x.Login == login);
        var hash = GetPasswordHash(password, account.Salt, account.IsPasswordKeptAsHash);
        return hash == account.PasswordHash;
    }

    private string GenerateSalt()
    {
        // Generate a 128-bit salt using a sequence of
        // cryptographically strong random bytes.
        var salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
        return Convert.ToBase64String(salt);
    }

    private string GetPasswordHash(string password, string salt, bool isPasswordKeptAsHash)
    {
        return isPasswordKeptAsHash
            ? _hashService.HashWithSHA512(password + salt)
            : _hashService.HashWithHMAC(password, salt);
    }

    private byte[] GetMasterPasswordBytes(string password)
    {
        return Convert.FromBase64String(
            _hashService.HashWithMD5(_hashService.HashWithHMAC(password, _configuration["Pepper"])));
    }
}