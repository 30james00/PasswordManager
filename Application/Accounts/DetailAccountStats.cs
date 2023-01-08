using MediatR;
using PasswordManager.Application.Core;
using PasswordManager.Application.LoginAttempts;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public record DetailAccountStatsQuery(Guid AccountId) : IRequest<ApiResult<string>>;

public class DetailAccountStatsQueryHandler : IRequestHandler<DetailAccountStatsQuery, ApiResult<string>>
{
    private readonly ILoginAttemptsService _loginAttemptsService;
    private readonly IUserAccessor _userAccessor;

    public DetailAccountStatsQueryHandler(ILoginAttemptsService loginAttemptsService, IUserAccessor userAccessor)
    {
        _loginAttemptsService = loginAttemptsService;
        _userAccessor = userAccessor;
    }

    public async Task<ApiResult<string>> Handle(DetailAccountStatsQuery request, CancellationToken cancellationToken)
    {
        var successfulLoginTime = await _loginAttemptsService.LastSuccessfulLoginAttemptTime(request.AccountId);
        var unsuccessfulLoginTime = await _loginAttemptsService.LastUnsuccessfulLoginAttemptTime(request.AccountId);

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