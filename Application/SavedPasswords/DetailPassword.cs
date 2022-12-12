using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.SavedPasswords.DAOs;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SavedPasswords;

public record DetailPasswordQuery(Guid Id) : IRequest<ApiResult<SavedPasswordDao>>;

public class DetailPasswordQueryHandler : IRequestHandler<DetailPasswordQuery, ApiResult<SavedPasswordDao>>
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

    public async Task<ApiResult<SavedPasswordDao>> Handle(DetailPasswordQuery request,
        CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<SavedPasswordDao>.Forbidden();
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(accountId), cancellationToken);
        if (account == null) return ApiResult<SavedPasswordDao>.Forbidden();

        // Get SavedPassword
        var savedPassword =
            await _dataContext.SavedPasswords.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (savedPassword == null) return ApiResult<SavedPasswordDao>.Failure("Chosen Password does not exist");

        //Authorize owner of Password
        if (account.Id != savedPassword.AccountId) return ApiResult<SavedPasswordDao>.Forbidden();

        return ApiResult<SavedPasswordDao>.Success(_mapper.Map<SavedPassword, SavedPasswordDao>(savedPassword));
    }
}