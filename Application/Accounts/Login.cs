using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.Core;
using PasswordManager.Application.LoginAttempts;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public record LoginQuery(string Login, string Password) : IRequest<ApiResult<AccountDto>>;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ApiResult<AccountDto>>
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly IAccountService _accountService;
    private readonly ILoginAttemptsService _loginAttemptsService;
    private readonly IUserAccessor _userAccessor;

    public LoginQueryHandler(DataContext dataContext, ITokenService tokenService,
        IAccountService accountService, ILoginAttemptsService loginAttemptsService, IUserAccessor userAccessor)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
        _accountService = accountService;
        _loginAttemptsService = loginAttemptsService;
        _userAccessor = userAccessor;
    }

    public async Task<ApiResult<AccountDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken);
        if (account == null) return ApiResult<AccountDto>.Forbidden();

        var ipAddress = _userAccessor.GetRequestIpAddress();

        var blockLogInTime = await _loginAttemptsService.ThrottleLogInTime(account.Id);
        var blockIpAddressTime = await _loginAttemptsService.ThrottleIpLogIn(account.Id, ipAddress);
        if (blockIpAddressTime == int.MaxValue)
        {
            await _loginAttemptsService.LogLoginAttempt(account.Id, false, ipAddress);
            return ApiResult<AccountDto>.Forbidden("Your IP address is blocked");
        }

        var blockTime = blockIpAddressTime > blockLogInTime ? blockIpAddressTime : blockLogInTime;
        if (blockTime > 0)
        {
            var lastUnsuccessfulDateTime =
                await _loginAttemptsService.LastUnsuccessfulLoginAttemptTime(account.Id) ?? DateTime.Now;
            var blockTimeDelta = DateTime.Now.Subtract(lastUnsuccessfulDateTime).TotalSeconds;
            if (blockTimeDelta < blockTime)
                return ApiResult<AccountDto>.Forbidden(
                    $"Too many login attempts - account is locked for {(blockTime - blockTimeDelta):#.##} s");
        }

        var hash = _accountService.GetPasswordHash(request.Password, account.Salt,
            account.IsPasswordKeptAsHash);
        if (hash != account.PasswordHash)
        {
            await _loginAttemptsService.LogLoginAttempt(account.Id, false, ipAddress);
            return ApiResult<AccountDto>.Forbidden();
        }

        await _loginAttemptsService.LogLoginAttempt(account.Id, true, ipAddress);
        return ApiResult<AccountDto>.Success(new AccountDto
        {
            Login = request.Login,
            // Create JWT token
            Token = _tokenService.CreateToken(account),
        });
    }
}