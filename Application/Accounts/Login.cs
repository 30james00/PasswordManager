using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public record LoginQuery(string Login, string Password) : IRequest<ApiResult<AccountDto>>;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ApiResult<AccountDto>>
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

    public async Task<ApiResult<AccountDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken);
        if (account == null) return ApiResult<AccountDto>.Forbidden();

        var hash = _accountService.GetPasswordHash(request.Password, account.Salt,
            account.IsPasswordKeptAsHash);
        if (hash != account.PasswordHash) return ApiResult<AccountDto>.Forbidden();

        return ApiResult<AccountDto>.Success(new AccountDto
        {
            Login = request.Login,
            // Create JWT token
            Token = _tokenService.CreateToken(account),
        });
    }
}