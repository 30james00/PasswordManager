using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.SavedPasswords;

namespace PasswordManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordsController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<SavedPasswordDto>>> ListPasswords()
    {
        return Unauthorized();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SavedPasswordDto>> CreatePassword(SavedPassword password)
    {
        return Unauthorized();
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult<SavedPasswordDto>> EditPassword(SavedPassword password)
    {
        return Unauthorized();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePassword(Guid id)
    {
        return Unauthorized();
    }

    [Authorize]
    [HttpGet("decrypt/{id:guid}")]
    public async Task<ActionResult<string>> DecryptPassword(Guid id)
    {
        return Unauthorized();
    }
}