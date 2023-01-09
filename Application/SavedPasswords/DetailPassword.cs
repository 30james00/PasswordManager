using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SavedPasswords;

public record DetailPasswordQuery(Guid Id) : IRequest<ApiResult<SavedPasswordDto>>;

public class DetailPasswordQueryHandler : IRequestHandler<DetailPasswordQuery, ApiResult<SavedPasswordDto>>
{
    private readonly IUserAccessor _userAccessor;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public DetailPasswordQueryHandler(IUserAccessor userAccessor, DataContext dataContext, IMapper mapper)
    {
        _userAccessor = userAccessor;
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<ApiResult<SavedPasswordDto>> Handle(DetailPasswordQuery request,
        CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<SavedPasswordDto>.Forbidden();
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(accountId), cancellationToken);
        if (account == null) return ApiResult<SavedPasswordDto>.Forbidden();

        // Get SavedPassword
        return ApiResult<SavedPasswordDto>.Success(
            await _dataContext.SavedPasswords.Where(x => x.Account.Id == account.Id)
                .ProjectTo<SavedPasswordDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken));
    }
}