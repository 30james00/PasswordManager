using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SharedPasswords;

public record DecryptSharedPasswordQuery(Guid Id) : IRequest<ApiResult<string>>;

public class DecryptSharedPasswordQueryHandler : IRequestHandler<DecryptSharedPasswordQuery, ApiResult<string>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;
    private readonly IAccountService _accountService;
    private readonly ICryptoService _cryptoService;


    public DecryptSharedPasswordQueryHandler(DataContext dataContext, IUserAccessor userAccessor,
        IAccountService accountService, ICryptoService cryptoService)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
        _accountService = accountService;
        _cryptoService = cryptoService;
    }

    public async Task<ApiResult<string>> Handle(DecryptSharedPasswordQuery request,
        CancellationToken cancellationToken)
    {
        // Get AccountGuid
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<string>.Forbidden();
        var accountGuid = Guid.Parse(accountId);

        // Get SharedPassword
        var sharedPassword =
            await _dataContext.SharedPasswords.Include(x => x.SavedPassword)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (sharedPassword == null) return ApiResult<string>.Failure("Chosen Password does not exist");

        //Authorize owner of Password
        if (accountGuid != sharedPassword.AccountId) return ApiResult<string>.Forbidden();

        //Get Owner Account
        var owner = await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == sharedPassword.SavedPassword.AccountId,
            cancellationToken);
        if (owner == null) return ApiResult<string>.Failure("Owner Account does not exists");

        var key = _accountService.GetMasterPasswordKey(owner.PasswordHash);

        //Get IV
        var ivBytes = Convert.FromBase64String(sharedPassword.SavedPassword.Iv);

        return ApiResult<string>.Success(_cryptoService.Decrypt(sharedPassword.SavedPassword.Password, key, ivBytes));
    }
}