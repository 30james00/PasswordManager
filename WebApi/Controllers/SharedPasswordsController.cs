using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.SharedPasswords;

namespace PasswordManager.Controllers;

public class SharedPasswordsController : BaseApiController
{
    public SharedPasswordsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreateSharedPassword(CreateSharedPasswordCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteSharedPassword(DeleteSharedPasswordCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }
}