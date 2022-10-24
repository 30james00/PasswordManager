using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Core;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SavedPasswords;

public class EditPasswordCommand : IRequest<ApiResult<SavedPasswordDto>>
{
    [Required] public Guid Id { get; set; }
    [Required] public string Password { get; set; } = null!;
    [Required] public string WebAddress { get; set; } = null!;
    public string? Description { get; set; }
    public string? Login { get; set; }
}

public class EditPasswordCommandHandler : IRequestHandler<EditPasswordCommand, ApiResult<SavedPasswordDto>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;
    private readonly IAccountService _accountService;
    private readonly ICryptoService _cryptoService;
    private readonly IMapper _mapper;

    public EditPasswordCommandHandler(DataContext dataContext, IUserAccessor userAccessor,
        IAccountService accountService, ICryptoService cryptoService, IMapper mapper)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
        _accountService = accountService;
        _cryptoService = cryptoService;
        _mapper = mapper;
    }

    public async Task<ApiResult<SavedPasswordDto>> Handle(EditPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<SavedPasswordDto>.Forbidden();
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(accountId), cancellationToken);
        if (account == null) return ApiResult<SavedPasswordDto>.Forbidden();

        // Get SavedPassword
        var savedPassword =
            await _dataContext.SavedPasswords.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (savedPassword == null) return ApiResult<SavedPasswordDto>.Failure("Chosen Password does not exist");

        //Authorize owner of Password
        if (account.Id != savedPassword.AccountId) return ApiResult<SavedPasswordDto>.Forbidden();

        // Create key for password encryption using MasterPassword hash
        var key = _accountService.GetMasterPasswordKey(account.PasswordHash);
        // Create IV for AES
        using var aes = Aes.Create();
        var ivBytes = aes.IV;

        // Edit entity fields
        savedPassword.Password = _cryptoService.Encrypt(request.Password, key, ivBytes);
        savedPassword.WebAddress = request.WebAddress;
        savedPassword.Description = request.Description;
        savedPassword.Login = request.Login;
        savedPassword.Iv = Convert.ToBase64String(ivBytes);

        await _dataContext.SavedPasswords.AddAsync(savedPassword, cancellationToken);
        var result = await _dataContext.SaveChangesAsync(cancellationToken) > 0;
        return result
            ? ApiResult<SavedPasswordDto>.Success(_mapper.Map<SavedPassword, SavedPasswordDto>(savedPassword))
            : ApiResult<SavedPasswordDto>.Failure("Error editing Password in Database");
    }
}