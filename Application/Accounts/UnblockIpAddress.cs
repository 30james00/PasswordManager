using MediatR;
using PasswordManager.Application.Core;
using PasswordManager.Application.LoginAttempts;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public record UnblockIpAddressCommand(string IpAddress) : IRequest<ApiResult<Unit>>;

public class UnblockIpAddressCommandHandler : IRequestHandler<UnblockIpAddressCommand, ApiResult<Unit>>
{
    private readonly IUserAccessor _userAccessor;
    private readonly ILoginAttemptsService _loginAttemptsService;

    public UnblockIpAddressCommandHandler(IUserAccessor userAccessor, ILoginAttemptsService loginAttemptsService)
    {
        _userAccessor = userAccessor;
        _loginAttemptsService = loginAttemptsService;
    }
    
    public async Task<ApiResult<Unit>> Handle(UnblockIpAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetUserId();
        if (userId == null) return ApiResult<Unit>.Forbidden();

        var accountId = Guid.Parse(userId);

        await _loginAttemptsService.UnlockIpAddress(accountId, request.IpAddress);
        
        return ApiResult<Unit>.Success(Unit.Value);
    }
}