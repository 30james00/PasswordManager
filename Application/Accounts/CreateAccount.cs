using System.ComponentModel.DataAnnotations;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.Core;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.Accounts;

public class CreateAccountCommand : IRequest<ApiResult<AccountDto>>
{
    [Required] public string Login { get; set; } = null!;

    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
    public string Password { get; set; } = null!;

    [Required] public bool IsPasswordKeptAsHash { get; set; }
}

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, ApiResult<AccountDto>>
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly IAccountService _accountService;

    public CreateAccountCommandHandler(DataContext dataContext, ITokenService tokenService,
        IAccountService accountService)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
        _accountService = accountService;
    }

    public async Task<ApiResult<AccountDto>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken) != null)
            return ApiResult<AccountDto>.Failure("Login already user");

        // Generate new salt and hash
        var salt = _accountService.GenerateSalt();
        var passwordHash = _accountService.GetPasswordHash(request.Password, salt, request.IsPasswordKeptAsHash);
        var account = new Account
        {
            Login = request.Login,
            PasswordHash = passwordHash,
            IsPasswordKeptAsHash = request.IsPasswordKeptAsHash,
            Salt = salt,
        };

        await _dataContext.Accounts.AddAsync(account, cancellationToken);
        var result = await _dataContext.SaveChangesAsync(cancellationToken) > 0;
        return result
            ? ApiResult<AccountDto>.Success(new AccountDto
            {
                Login = account.Login,
                Token = _tokenService.CreateToken(account),
            })
            : ApiResult<AccountDto>.Failure("Failed to add Account to DB");
    }
}