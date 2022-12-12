using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts.DAOs;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public record LoginQuery(string Login, string Password) : IRequest<ApiResult<AccountDao>>;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ApiResult<AccountDao>>
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly IAccountService _accountService;

    public LoginQueryHandler(DataContext dataContext, ITokenService tokenService,
        IAccountService accountService)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
        _accountService = accountService;
    }

    public async Task<ApiResult<AccountDao>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken);
        if (account == null) return ApiResult<AccountDao>.Forbidden();

        var hash = _accountService.GetPasswordHash(request.Password, account.Salt,
            account.IsPasswordKeptAsHash);
        if (hash != account.PasswordHash) return ApiResult<AccountDao>.Forbidden();

        return ApiResult<AccountDao>.Success(new AccountDao
        {
            Login = request.Login,
            // Create JWT token
            Token = _tokenService.CreateToken(account),
        });
    }
}