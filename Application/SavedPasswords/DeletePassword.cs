using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SavedPasswords;

public record DeletePasswordCommand(Guid Id) : IRequest<ApiResult<Unit>>;

public class DeletePasswordCommandHandler : IRequestHandler<DeletePasswordCommand, ApiResult<Unit>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;

    public DeletePasswordCommandHandler(DataContext dataContext, IUserAccessor userAccessor)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(DeletePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<Unit>.Forbidden();
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(accountId), cancellationToken);
        if (account == null) return ApiResult<Unit>.Forbidden();

        // Get SavedPassword
        var savedPassword =
            await _dataContext.SavedPasswords.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (savedPassword == null) return ApiResult<Unit>.Failure("Chosen Password does not exist");

        //Authorize owner of Password
        if (account.Id != savedPassword.AccountId) return ApiResult<Unit>.Forbidden();

        _dataContext.SavedPasswords.Remove(savedPassword);
        var result = await _dataContext.SaveChangesAsync(cancellationToken) > 0;
        return result
            ? ApiResult<Unit>.Success(Unit.Value)
            : ApiResult<Unit>.Failure("Error saving new Password to Database");
    }
}