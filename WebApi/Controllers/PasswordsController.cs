using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.SavedPasswords;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordsController : ControllerBase
{
    private readonly ISavedPasswordService _savedPasswordService;
    private readonly IUserAccessor _userAccessor;

    public PasswordsController(ISavedPasswordService savedPasswordService, IUserAccessor userAccessor)
    {
        _savedPasswordService = savedPasswordService;
        _userAccessor = userAccessor;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<SavedPasswordDto>>> ListPasswords()
    {
        return await _savedPasswordService.ListPassword();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SavedPasswordDto>> CreatePassword(CreatePasswordDto passwordDto)
    {
        return await _savedPasswordService.CreatePassword(passwordDto);
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult<SavedPasswordDto>> EditPassword(EditPasswordDto passwordDto)
    {
        var password = await _savedPasswordService.DetailPassword(passwordDto.Id);
        var accountId = _userAccessor.GetUserId();
        if (password == null) return NotFound();
        if (accountId == null) throw new KeyNotFoundException("Account not found");
        if (password.AccountId != Guid.Parse(accountId)) return Unauthorized();

        return await _savedPasswordService.EditPassword(passwordDto);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePassword(Guid id)
    {
        var password = await _savedPasswordService.DetailPassword(id);
        var accountId = _userAccessor.GetUserId();
        if (password == null) return NotFound();
        if (accountId == null) throw new KeyNotFoundException("Account not found");
        if (password.AccountId != Guid.Parse(accountId)) return Unauthorized();

        await _savedPasswordService.DeletePassword(id);
        return Ok();
    }

    [Authorize]
    [HttpGet("decrypt/{id:guid}")]
    public async Task<ActionResult<string>> DecryptPassword(Guid id)
    {
        var password = await _savedPasswordService.DetailPassword(id);
        var accountId = _userAccessor.GetUserId();
        if (password == null) return NotFound();
        if (accountId == null) throw new KeyNotFoundException("Account not found");
        if (password.AccountId != Guid.Parse(accountId)) return Unauthorized();

        return await _savedPasswordService.DecryptPassword(id);
    }
}