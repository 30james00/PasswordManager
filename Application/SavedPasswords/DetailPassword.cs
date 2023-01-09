using AutoMapper;
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
        var savedPassword =
            await _dataContext.SavedPasswords.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (savedPassword == null) return ApiResult<SavedPasswordDto>.Failure("Chosen Password does not exist");

        // Authorize owner of Password
        if (account.Id != savedPassword.AccountId) return ApiResult<SavedPasswordDto>.Forbidden();

        return ApiResult<SavedPasswordDto>.Success(_mapper.Map<SavedPassword, SavedPasswordDto>(savedPassword));
    }
}