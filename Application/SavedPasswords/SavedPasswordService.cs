using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;
using Aes = System.Security.Cryptography.Aes;

namespace PasswordManager.Application.SavedPasswords;

public class SavedPasswordService : ISavedPasswordService
{
    private readonly DataContext _dataContext;
    private readonly ICryptoService _cryptoService;
    private readonly IHashService _hashService;
    private readonly IUserAccessor _userAccessor;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<SavedPasswordService> _logger;

    public SavedPasswordService(DataContext dataContext, ICryptoService cryptoService, IHashService hashService,
        IUserAccessor userAccessor, IConfiguration configuration, IMapper mapper, ILogger<SavedPasswordService> logger)
    {
        _dataContext = dataContext;
        _cryptoService = cryptoService;
        _hashService = hashService;
        _userAccessor = userAccessor;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<SavedPasswordDto>> ListPassword()
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) throw new KeyNotFoundException("User not logged in");
        
        return await _dataContext.SavedPasswords.Where(x => x.AccountId == Guid.Parse(accountId))
            .ProjectTo<SavedPasswordDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<SavedPasswordDto?> DetailPassword(Guid id)
    {
        return await _dataContext.SavedPasswords.ProjectTo<SavedPasswordDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<SavedPasswordDto> CreatePassword(CreatePasswordDto passwordDto)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) throw new KeyNotFoundException("User not logged in");
        var account = await _dataContext.Accounts.FirstAsync(x => x.Id == Guid.Parse(accountId));

        //Create key for password encryption using MasterPassword hash
        var key = CreateMasterPasswordKey(account.PasswordHash);
        // Create IV for AES
        using var aes = Aes.Create();
        var ivBytes = aes.IV;
        var savedPassword = new SavedPassword
        {
            // Encrypt password
            Password = _cryptoService.Encrypt(passwordDto.Password, key, ivBytes),
            WebAddress = passwordDto.WebAddress,
            Description = passwordDto.Description,
            Login = passwordDto.Login,
            Iv = Convert.ToBase64String(ivBytes),
            AccountId = Guid.Parse(accountId),
        };

        await _dataContext.SavedPasswords.AddAsync(savedPassword);
        var result = await _dataContext.SaveChangesAsync();
        if (result <= 0) throw new Exception("Error saving new Password to Database");
        return _mapper.Map<SavedPassword, SavedPasswordDto>(savedPassword);
    }

    public async Task<SavedPasswordDto> EditPassword(EditPasswordDto passwordDto)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) throw new KeyNotFoundException("User not logged in");
        var account = await _dataContext.Accounts.FirstAsync(x => x.Id == Guid.Parse(accountId));

        // Get SavedPassword
        var savedPassword = await _dataContext.SavedPasswords.FirstAsync(x => x.Id == passwordDto.Id);
        
        // Create key for password encryption using MasterPassword hash
        var key = CreateMasterPasswordKey(account.PasswordHash);
        // Create IV for AES
        using var aes = Aes.Create();
        var ivBytes = aes.IV;

        // Edit entity fields
        savedPassword.Password = _cryptoService.Encrypt(passwordDto.Password, key, ivBytes);
        savedPassword.WebAddress = passwordDto.WebAddress;
        savedPassword.Description = passwordDto.Description;
        savedPassword.Login = passwordDto.Login;
        savedPassword.Iv = Convert.ToBase64String(ivBytes);

        var result = await _dataContext.SaveChangesAsync();
        if (result <= 0) throw new Exception("Error saving new Password to Database");
        return _mapper.Map<SavedPassword, SavedPasswordDto>(savedPassword);
    }

    public async Task DeletePassword(Guid id)
    {
        // Get SavedPassword
        var savedPassword = await _dataContext.SavedPasswords.FirstAsync(x => x.Id == id);

        _dataContext.SavedPasswords.Remove(savedPassword);
        var result = await _dataContext.SaveChangesAsync();
        if (result <= 0) throw new Exception("Error saving new Password to Database");
    }
    
    public async Task<string> DecryptPassword(Guid id)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) throw new KeyNotFoundException("User not logged in");
        var account = await _dataContext.Accounts.FirstAsync(x => x.Id == Guid.Parse(accountId));
        // Get SavedPassword
        var savedPassword = await _dataContext.SavedPasswords.FirstAsync(x => x.Id == id);
        if (savedPassword == null) throw new KeyNotFoundException("SavedPassword not found");
        var key = CreateMasterPasswordKey(account.PasswordHash);

        //Get IV
        var ivBytes = Convert.FromBase64String(savedPassword.Iv);
        return _cryptoService.Decrypt(savedPassword.Password, key, ivBytes);
    }

    /// <summary>
    /// Creates Key for Encryption using MD-5 hash of MasterPassword and pepper HMAC value
    /// </summary>
    /// <param name="password">MasterPassword</param>
    /// <returns>128-bit key</returns>
    private byte[] CreateMasterPasswordKey(string password)
    {
        return Convert.FromBase64String(
            _hashService.HashWithMD5(_hashService.HashWithHMAC(password, _configuration["Pepper"])));
    }
}