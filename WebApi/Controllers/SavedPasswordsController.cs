using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.SavedPasswords;
using PasswordManager.Application.SavedPasswords.DTOs;

namespace PasswordManager.Controllers;

public class SavedPasswordsController : BaseApiController
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<SavedPasswordDto>>> ListPasswords()
    {
        return HandleResult(await Mediator.Send(new ListPasswordQuery()));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SavedPasswordDto>> CreatePassword(CreatePasswordCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult<SavedPasswordDto>> EditPassword(EditPasswordCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePassword(DeletePasswordCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [Authorize]
    [HttpGet("decrypt/{id:guid}")]
    public async Task<ActionResult<string>> DecryptPassword(Guid id)
    {
        return HandleResult(await Mediator.Send(new DecryptPasswordQuery(id)));
    }
}