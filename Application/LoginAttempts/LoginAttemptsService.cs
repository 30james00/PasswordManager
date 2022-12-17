using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.LoginAttempts;

public class LoginAttemptsService : ILoginAttemptsService
{
    private readonly DataContext _context;
    private readonly Guid _accountId;

    public LoginAttemptsService(DataContext context, Guid accountId)
    {
        _context = context;
        _accountId = accountId;
    }

    public async Task LogLoginAttempt(bool isSuccessful, string ipAddress)
    {
        await _context.LoginAttempts.AddAsync(new LoginAttempt
        {
            AccountId = _accountId,
            Time = new DateTime(),
            IpAddress = ipAddress,
            IsSuccessful = isSuccessful,
        });
        await _context.SaveChangesAsync();
    }

    public DateTime? LastSuccessfulLoginAttemptTime()
    {
        return _context.LoginAttempts.Where(x=> x.AccountId == _accountId && x.IsSuccessful == true).OrderByDescending(x => x.Time).FirstOrDefault()
            ?.Time;
    }

    public DateTime? LastUnsuccessfulLoginAttemptTime()
    {
        return _context.LoginAttempts.Where(x => x.AccountId == _accountId && x.IsSuccessful == false).OrderByDescending(x => x.Time).FirstOrDefault()
            ?.Time;
    }

    public void ThrottleLogIn()
    {
        throw new NotImplementedException();
    }

    public void ResetLocks()
    {
        throw new NotImplementedException();
    }
}