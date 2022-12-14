using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.IpAddressBlocks;
using PasswordManager.Application.IpAddressBlocks.DTOs;

namespace PasswordManager.Controllers;

public class AccountController : BaseApiController
{
    public AccountController(IMediator mediator) : base(mediator)
    {
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AccountDto>> Login(LoginQuery loginQuery)
    {
        return HandleResult(await _mediator.Send(loginQuery));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AccountDto>> Register(CreateAccountCommand createAccountCommand)
    {
        return HandleResult(await _mediator.Send(createAccountCommand));
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<ActionResult<AccountDto>> ChangePassword(ChangePasswordCommand changePasswordCommand)
    {
        return HandleResult(await _mediator.Send(changePasswordCommand));
    }

    [AllowAnonymous]
    [HttpGet("login-stats/{login}")]
    public async Task<ActionResult<string>> LoginStats(string login)
    {
        return HandleResult(await _mediator.Send(new DetailAccountStatsQuery(login)));
    }

    [Authorize]
    [HttpGet("ip-block")]
    public async Task<ActionResult<List<IpAddressBlockDto>>> ListIpAddressBlocks()
    {
        return HandleResult(await _mediator.Send(new ListIpAddressBlocksQuery()));
    }

    [Authorize]
    [HttpDelete("ip-block/{id:guid}")]
    public async Task<ActionResult> RemoveIpBlock(Guid id)
    {
        return HandleResult(await _mediator.Send(new UnblockIpAddressCommand(id)));
    }
}