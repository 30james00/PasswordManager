using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SharedPasswords;

public record DeleteSharedPasswordCommand(Guid Id, string Login) : IRequest<ApiResult<Unit>>;

public class DeleteSharedPasswordCommandHandler : IRequestHandler<DeleteSharedPasswordCommand, ApiResult<Unit>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;

    public DeleteSharedPasswordCommandHandler(DataContext dataContext, IUserAccessor userAccessor)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(DeleteSharedPasswordCommand request, CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<Unit>.Forbidden();
        var owner =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(accountId), cancellationToken);
        if (owner == null) return ApiResult<Unit>.Forbidden();

        // Find user to share password
        var toAccount =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken);
        if (toAccount == null) return ApiResult<Unit>.Failure("User does not exist");

        // Get SharedPassword
        var sharedPassword =
            await _dataContext.SharedPasswords.Include(x => x.Account)
                .Include(x => x.SavedPassword.Account)
                .FirstOrDefaultAsync(
                    x => x.SavedPasswordId == request.Id && x.AccountId == toAccount.Id, cancellationToken);
        if (sharedPassword == null) return ApiResult<Unit>.Failure("Shared Password does not exist");

        // Authorize owner of Password
        if (owner.Id != sharedPassword.SavedPassword.Account.Id) return ApiResult<Unit>.Forbidden();

        // Save to Database
        _dataContext.SharedPasswords.Remove(sharedPassword);
        var result = await _dataContext.SaveChangesAsync(cancellationToken) > 0;
        return result ? ApiResult<Unit>.Success(Unit.Value) : ApiResult<Unit>.Failure("Error sharing password");
    }
}