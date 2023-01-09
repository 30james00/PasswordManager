using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Token;
using PasswordManager.Application.SharedPasswords.DTOs;

namespace PasswordManager.Application.SharedPasswords;

public record ListSharedPasswordQuery : IRequest<ApiResult<List<SharedPasswordDto>>>;

public class
    ListSharedPasswordQueryHandler : IRequestHandler<ListSharedPasswordQuery, ApiResult<List<SharedPasswordDto>>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public ListSharedPasswordQueryHandler(DataContext dataContext, IUserAccessor userAccessor, IMapper mapper)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<SharedPasswordDto>>> Handle(ListSharedPasswordQuery request,
        CancellationToken cancellationToken)
    {
        // Get AccountGuid
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<List<SharedPasswordDto>>.Forbidden();
        var accountGuid = Guid.Parse(accountId);

        return ApiResult<List<SharedPasswordDto>>.Success(await _dataContext.SharedPasswords
            .Where(x => x.AccountId == accountGuid)
            .ProjectTo<SharedPasswordDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken));
    }
}