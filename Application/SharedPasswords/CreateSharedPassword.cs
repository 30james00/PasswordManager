using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SharedPasswords;

public record CreateSharedPasswordCommand(Guid Id, string Login) : IRequest<ApiResult<Unit>>;

public class CreateSharedPasswordCommandHandler : IRequestHandler<CreateSharedPasswordCommand, ApiResult<Unit>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;

    public CreateSharedPasswordCommandHandler(DataContext dataContext, IUserAccessor userAccessor)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(CreateSharedPasswordCommand request, CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<Unit>.Forbidden();
        var owner =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(accountId), cancellationToken);
        if (owner == null) return ApiResult<Unit>.Forbidden();

        // Check if user shares to himself
        if (owner.Login == request.Login) return ApiResult<Unit>.Failure("You can't share password with yourself");

        // Get SavedPassword
        var savedPassword =
            await _dataContext.SavedPasswords.Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == request.Id,
                    cancellationToken);
        if (savedPassword == null) return ApiResult<Unit>.Failure("SavedPassword does not exist");

        // Authorize owner of Password
        if (owner.Id != savedPassword.AccountId) return ApiResult<Unit>.Forbidden();

        var toAccount =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken);
        if (toAccount == null) return ApiResult<Unit>.Failure("User does not exist");

        // Check if already shared
        if (owner.SavedPasswords.Any(x => x.Id == request.Id && x.Account.Login == request.Login))
            return ApiResult<Unit>.Failure("Password already shared with this user");

        // Save to Database
        await _dataContext.SharedPasswords.AddAsync(new SharedPassword
        {
            AccountId = toAccount.Id,
            SavedPasswordId = savedPassword.Id,
        }, cancellationToken);
        var result = await _dataContext.SaveChangesAsync(cancellationToken) > 0;
        return result ? ApiResult<Unit>.Success(Unit.Value) : ApiResult<Unit>.Failure("Error sharing password");
    }
}