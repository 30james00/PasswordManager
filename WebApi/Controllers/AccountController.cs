using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Accounts.DTOs;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IUserAccessor _userAccessor;

    public AccountController(IAccountService accountService, IUserAccessor userAccessor)
    {
        _accountService = accountService;
        _userAccessor = userAccessor;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AccountDto>> Login(LoginDto loginDto)
    {
        if (!await _accountService.IfAccountExists(loginDto.Login)) return Unauthorized();

        if (!await _accountService.CheckPassword(loginDto.Login, loginDto.Password)) return Unauthorized();

        return await _accountService.Login(loginDto);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AccountDto>> Register(RegisterDto registerDto)
    {
        if (await _accountService.IfAccountExists(registerDto.Login)) return BadRequest("Login already used");

        return await _accountService.CreateAccount(registerDto);
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<ActionResult<AccountDto>> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var userId = _userAccessor.GetUserId();
        if (userId == null) return Unauthorized();
        if (!await _accountService.CheckPassword(Guid.Parse(userId), changePasswordDto.OldPassword))
            return Unauthorized();
        await _accountService.ChangePassword(Guid.Parse(userId), changePasswordDto.NewPassword,
            changePasswordDto.IsPasswordKeptAsHash);
        return Ok();
    }
}