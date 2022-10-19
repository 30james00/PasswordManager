using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.DTOs.Account;

namespace PasswordManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AccountDto>> Login(LoginDto loginDto)
    {
        return Unauthorized();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AccountDto>> Register(RegisterDto loginDto)
    {
        return Unauthorized();
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<ActionResult<AccountDto>> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        return Unauthorized();
    }
}