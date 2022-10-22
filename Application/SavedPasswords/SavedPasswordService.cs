using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SavedPasswords;

public class SavedPasswordService : ISavedPasswordService
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public SavedPasswordService(DataContext dataContext, IUserAccessor userAccessor, IMapper mapper)
    {
        _dataContext = dataContext;
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
        throw new NotImplementedException();
    }

    public async Task<SavedPasswordDto> CreatePassword()
    {
        throw new NotImplementedException();
    }

    public async Task<SavedPasswordDto> EditPassword()
    {
        throw new NotImplementedException();
    }

    public async Task DeletePassword()
    {
        throw new NotImplementedException();
    }
}