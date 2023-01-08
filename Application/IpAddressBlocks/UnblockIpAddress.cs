using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.IpAddressBlocks;

public record UnblockIpAddressCommand(string IpAddress) : IRequest<ApiResult<Unit>>;

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
        var userId = _userAccessor.GetUserId();
        if (userId == null) return ApiResult<Unit>.Forbidden();

        var accountId = Guid.Parse(userId);

        var block = await _context.IpAddressBlocks.FirstOrDefaultAsync(x =>
            x.IpAddress == request.IpAddress && x.AccountId == accountId, cancellationToken: cancellationToken);
        if (block != null)
        {
            _context.IpAddressBlocks.Remove(block);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return ApiResult<Unit>.Success(Unit.Value);
    }
}