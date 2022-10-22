using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SavedPasswords;

public class SavedPasswordService : ISavedPasswordService
{
    private readonly DataContext _dataContext;
    private readonly ICryptoService _cryptoService;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public SavedPasswordService(DataContext dataContext, ICryptoService cryptoService, IUserAccessor userAccessor,
        IMapper mapper)
    {
        _dataContext = dataContext;
        _cryptoService = cryptoService;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<List<SavedPasswordDto>> ListPassword()
    {
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

    public async Task<string> DecryptPassword(Guid id, string masterPassword)
    {
        var savedPassword = await _dataContext.SavedPasswords.FirstOrDefaultAsync(x => x.Id == id);
        if (savedPassword == null) throw new KeyNotFoundException("SavedPassword not found");
        var masterPasswordBytes = System.Text.Encoding.UTF8.GetBytes(masterPassword);
        var ivBytes = System.Text.Encoding.UTF8.GetBytes(savedPassword.Iv);
        return _cryptoService.Decrypt(savedPassword.Password, masterPasswordBytes, ivBytes);
    }

    public async Task<SavedPasswordDto> CreatePassword(CreatePasswordDto passwordDto)
    {
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) throw new KeyNotFoundException("User not logged in");
        var masterPasswordBytes = System.Text.Encoding.UTF8.GetBytes(passwordDto.MasterPassword);
        var ivBytes = System.Text.Encoding.UTF8.GetBytes("");
        var savedPassword = new SavedPassword
        {
            AccountId = Guid.Parse(accountId),
            Password = _cryptoService.Encrypt(passwordDto.Password, masterPasswordBytes, ivBytes),
            WebAddress = passwordDto.WebAddress,
            Description = passwordDto.Description,
            Login = passwordDto.Login,
            Iv = ivBytes.Aggregate("", (current, hashByte) => current + $"{hashByte:x2}"),
        };
        await _dataContext.SavedPasswords.AddAsync(savedPassword);
        var result = await _dataContext.SaveChangesAsync();
        if (result <= 0) throw new Exception("Error saving new Password to Database");
        return _mapper.Map<SavedPassword, SavedPasswordDto>(savedPassword);
    }

    public async Task<SavedPasswordDto> EditPassword(EditPasswordDto passwordDto)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePassword()
    {
        throw new NotImplementedException();
    }
}