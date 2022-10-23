using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Controllers;

public class AccountController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AccountDto>> Login(LoginQuery loginQuery)
    {
        return HandleResult(await Mediator.Send(loginQuery));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AccountDto>> Register(CreateAccountCommand createAccountCommand)
    {
        return HandleResult(await Mediator.Send(createAccountCommand));
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<ActionResult<AccountDto>> ChangePassword(ChangePasswordCommand changePasswordCommand)
    {
        return HandleResult(await Mediator.Send(changePasswordCommand));
    }
}