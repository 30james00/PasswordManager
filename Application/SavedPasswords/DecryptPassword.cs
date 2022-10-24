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

public record DecryptPasswordQuery(Guid Id) : IRequest<ApiResult<string>>;

public class DecryptPasswordQueryHandler : IRequestHandler<DecryptPasswordQuery, ApiResult<string>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;
    private readonly IAccountService _accountService;
    private readonly ICryptoService _cryptoService;


    public DecryptPasswordQueryHandler(DataContext dataContext, IUserAccessor userAccessor,
        IAccountService accountService, ICryptoService cryptoService)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
        _accountService = accountService;
        _cryptoService = cryptoService;
    }

    public async Task<ApiResult<string>> Handle(DecryptPasswordQuery request,
        CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<string>.Forbidden();
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(accountId), cancellationToken);
        if (account == null) return ApiResult<string>.Forbidden();

        // Get SavedPassword
        var savedPassword =
            await _dataContext.SavedPasswords.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (savedPassword == null) return ApiResult<string>.Failure("Chosen Password does not exist");

        //Authorize owner of Password
        if (account.Id != savedPassword.AccountId) return ApiResult<string>.Forbidden();

        var key = _accountService.GetMasterPasswordKey(account.PasswordHash);

        //Get IV
        var ivBytes = Convert.FromBase64String(savedPassword.Iv);

        return ApiResult<string>.Success(_cryptoService.Decrypt(savedPassword.Password, key, ivBytes));
    }
}