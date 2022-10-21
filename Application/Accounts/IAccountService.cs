using PasswordManager.Application.Accounts.DTOs;

namespace PasswordManager.Application.Accounts;

public interface IAccountService
{
    public Task<AccountDto> CreateAccount(RegisterDto registerDto);
    public Task<AccountDto> Login(LoginDto loginDto);
    public Task ChangePassword(ChangePasswordDto changePasswordDto);
    public Task<bool> IfAccountExists(string login);
    public Task<bool> CheckPassword(Guid id, string password);
    public Task<bool> CheckPassword(string login, string password);
}