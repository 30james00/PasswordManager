using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.IpAddressBlocks;

public record UnblockIpAddressCommand(Guid Id) : IRequest<ApiResult<Unit>>;

public class UnblockIpAddressCommandHandler : IRequestHandler<UnblockIpAddressCommand, ApiResult<Unit>>
{
    private readonly IUserAccessor _userAccessor;
    private readonly DataContext _context;

    public UnblockIpAddressCommandHandler(IUserAccessor userAccessor, DataContext context)
    {
        _userAccessor = userAccessor;
        _context = context;
    }

    public async Task<ApiResult<Unit>> Handle(UnblockIpAddressCommand request, CancellationToken cancellationToken)
    {
        // Get Account Id
        var userId = _userAccessor.GetUserId();
        if (userId == null) return ApiResult<Unit>.Forbidden();
        var accountId = Guid.Parse(userId);

        // Get blocked IP
        var block = await _context.IpAddressBlocks.FirstOrDefaultAsync(x =>
            x.Id == request.Id, cancellationToken: cancellationToken);
        if (block == null) return ApiResult<Unit>.Failure("Chosen IP block does not exist");

        // Authorize owner of Blocked IP
        if (block.AccountId != accountId) return ApiResult<Unit>.Forbidden();

        _context.IpAddressBlocks.Remove(block);
        var result = await _context.SaveChangesAsync(cancellationToken) > 0;

        return result
            ? ApiResult<Unit>.Success(Unit.Value)
            : ApiResult<Unit>.Failure("Error removing IP block from database");
    }
}