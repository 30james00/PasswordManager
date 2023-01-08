using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.LoginAttempts;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public record DetailAccountStatsQuery(string Login) : IRequest<ApiResult<string>>;

public class DetailAccountStatsQueryHandler : IRequestHandler<DetailAccountStatsQuery, ApiResult<string>>
{
    private readonly DataContext _context;
    private readonly ILoginAttemptsService _loginAttemptsService;
    private readonly IUserAccessor _userAccessor;

    public DetailAccountStatsQueryHandler(DataContext context, ILoginAttemptsService loginAttemptsService,
        IUserAccessor userAccessor)
    {
        _context = context;
        _loginAttemptsService = loginAttemptsService;
        _userAccessor = userAccessor;
    }

    public async Task<ApiResult<string>> Handle(DetailAccountStatsQuery request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken);
        if (account == null) return ApiResult<string>.Failure("Account does not exist");

        var successfulLoginTime = await _loginAttemptsService.LastSuccessfulLoginAttemptTime(account.Id);
        var unsuccessfulLoginTime = await _loginAttemptsService.LastUnsuccessfulLoginAttemptTime(account.Id);

        var message = "";
        message += successfulLoginTime == null
            ? "No successful logins\n"
            : $"Last successful login at: {successfulLoginTime}\n";
        message += unsuccessfulLoginTime == null
            ? "No unsuccessful logins"
            : $"Last unsuccessful login at: {unsuccessfulLoginTime}";
        return ApiResult<string>.Success(message);
    }
}