using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.LoginAttempts.DTOs;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.IpAddressBlocks;

public record ListIpAddressBlocksQuery : IRequest<ApiResult<List<IpAddressBlockDto>>>;

public class
    ListIpAddressBlocksQueryHandler : IRequestHandler<ListIpAddressBlocksQuery, ApiResult<List<IpAddressBlockDto>>>
{
    private readonly DataContext _context;
    private readonly IUserAccessor _userAccessor;

    public ListIpAddressBlocksQueryHandler(DataContext context, IUserAccessor userAccessor)
    {
        _context = context;
        _userAccessor = userAccessor;
    }

    public async Task<ApiResult<List<IpAddressBlockDto>>> Handle(ListIpAddressBlocksQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetUserId();
        if (userId == null) return ApiResult<List<IpAddressBlockDto>>.Forbidden();
        var accountId = Guid.Parse(userId);

        return ApiResult<List<IpAddressBlockDto>>.Success(await _context.IpAddressBlocks
            .Where(x => x.AccountId == accountId).Cast<IpAddressBlockDto>()
            .ToListAsync(cancellationToken));
    }
}