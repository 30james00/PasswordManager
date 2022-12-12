using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts.DAOs;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public class ChangePasswordCommand : IRequest<ApiResult<AccountDao>>
{
    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
    public string OldPassword { get; set; } = null!;

    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
    public string NewPassword { get; set; } = null!;

    [Required] public bool IsPasswordKeptAsHash { get; set; }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResult<AccountDao>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;
    private readonly IAccountService _accountService;
    private readonly ICryptoService _cryptoService;
    private readonly ITokenService _tokenService;

    public ChangePasswordCommandHandler(DataContext dataContext, IUserAccessor userAccessor,
        IAccountService accountService, ICryptoService cryptoService,
        ITokenService tokenService)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
        _accountService = accountService;
        _cryptoService = cryptoService;
        _tokenService = tokenService;
    }

    public async Task<ApiResult<AccountDao>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetUserId();
        if (userId == null) return ApiResult<AccountDao>.Forbidden();
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId), cancellationToken);
        if (account == null) return ApiResult<AccountDao>.Forbidden();
        var hash = _accountService.GetPasswordHash(request.OldPassword, account.Salt, account.IsPasswordKeptAsHash);
        if (hash != account.PasswordHash) return ApiResult<AccountDao>.Forbidden();

        // Generate new salt and passwordHash
        var salt = _accountService.GenerateSalt();
        var passwordHash = _accountService.GetPasswordHash(request.NewPassword, salt, request.IsPasswordKeptAsHash);

        // Decrypt passwords encrypted with old MasterPassword
        var savedPasswords = await _dataContext.SavedPasswords.Where(x => x.AccountId == account.Id)
            .ToListAsync(cancellationToken);
        var key = _accountService.GetMasterPasswordKey(account.PasswordHash);
        foreach (var savedPassword in savedPasswords)
        {
            var ivBytes = Convert.FromBase64String(savedPassword.Iv);
            savedPassword.Password = _cryptoService.Decrypt(savedPassword.Password, key, ivBytes);
        }

        // Save new MasterPassword and salt
        account.PasswordHash = passwordHash;
        account.Salt = salt;
        account.IsPasswordKeptAsHash = request.IsPasswordKeptAsHash;

        // Encrypt passwords with new MasterPassword
        foreach (var savedPassword in savedPasswords)
        {
            var masterPasswordBytes = _accountService.GetMasterPasswordKey(passwordHash);
            savedPassword.Password = _cryptoService.Encrypt(savedPassword.Password, masterPasswordBytes, out var ivBytes);
            savedPassword.Iv = Convert.ToBase64String(ivBytes);
        }

        var result = await _dataContext.SaveChangesAsync(cancellationToken) > 0;
        return result
            ? ApiResult<AccountDao>.Success(new AccountDao
            {
                Login = account.Login,
                Token = _tokenService.CreateToken(account),
            })
            : ApiResult<AccountDao>.Failure("Error saving new MasterPassword to Database");
    }
}