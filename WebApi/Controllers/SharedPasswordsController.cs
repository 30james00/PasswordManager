using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.SharedPasswords;
using PasswordManager.Application.SharedPasswords.DTOs;

namespace PasswordManager.Controllers;

public class SharedPasswordsController : BaseApiController
{
    public SharedPasswordsController(IMediator mediator) : base(mediator)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<SharedPasswordDto>>> ListSharedPassword()
    {
        return HandleResult(await _mediator.Send(new ListSharedPasswordQuery()));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateSharedPassword(CreateSharedPasswordCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteSharedPassword(Guid id)
    {
        return HandleResult(await _mediator.Send(new DeleteSharedPasswordCommand(id)));
    }

    [Authorize]
    [HttpGet("decrypt/{id:guid}")]
    public async Task<ActionResult<string>> DecryptSharedPassword(Guid id)
    {
        return HandleResult(await _mediator.Send(new DecryptSharedPasswordQuery(id)));
    }
}