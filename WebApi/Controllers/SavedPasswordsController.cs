using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.SavedPasswords;
using PasswordManager.Application.SavedPasswords.DAOs;

namespace PasswordManager.Controllers;

public class SavedPasswordsController : BaseApiController
{
    public SavedPasswordsController(IMediator mediator) : base(mediator)
    {
    }
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<SavedPasswordDao>>> ListPasswords()
    {
        return HandleResult(await _mediator.Send(new ListPasswordQuery()));
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<List<SavedPasswordDao>>> DetailPassword(Guid id)
    {
        return HandleResult(await _mediator.Send(new DetailPasswordQuery(id)));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SavedPasswordDao>> CreatePassword(CreatePasswordCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult<SavedPasswordDao>> EditPassword(EditPasswordCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePassword(Guid id)
    {
        return HandleResult(await _mediator.Send(new DeletePasswordCommand(id)));
    }

    [Authorize]
    [HttpGet("decrypt/{id:guid}")]
    public async Task<ActionResult<string>> DecryptPassword(Guid id)
    {
        return HandleResult(await _mediator.Send(new DecryptPasswordQuery(id)));
    }
}