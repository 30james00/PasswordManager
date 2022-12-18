using MediatR;
using PasswordManager.Application.Accounts.DAOs;
using PasswordManager.Application.Core;
using PasswordManager.Application.LoginAttempts;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public record AccountStatsQuery : IRequest<ApiResult<string>>;

public class AccountStatsQueryHandler : IRequestHandler<AccountStatsQuery, ApiResult<string>>
{
    private readonly ILoginAttemptsService _loginAttemptsService;
    private readonly IUserAccessor _userAccessor;

    public AccountStatsQueryHandler(ILoginAttemptsService loginAttemptsService, IUserAccessor userAccessor)
    {
        _loginAttemptsService = loginAttemptsService;
        _userAccessor = userAccessor;
    }

    public async Task<ApiResult<string>> Handle(AccountStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetUserId();
        if (userId == null) return ApiResult<string>.Forbidden();

        var accountId = Guid.Parse(userId);

        var successfulLoginTime = await _loginAttemptsService.LastSuccessfulLoginAttemptTime(accountId);
        var unsuccessfulLoginTime = await _loginAttemptsService.LastUnsuccessfulLoginAttemptTime(accountId);

        var message = "";
        message += successfulLoginTime == null
            ? "No successful logins"
            : $"Last successful login at: {successfulLoginTime}";
        message += unsuccessfulLoginTime == null
            ? "No unsuccessful logins"
            : $"Last unsuccessful login at: {unsuccessfulLoginTime}";
        return ApiResult<string>.Success(message);
    }
}