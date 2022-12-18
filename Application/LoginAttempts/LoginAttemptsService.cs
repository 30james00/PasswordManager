using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace PasswordManager.Application.LoginAttempts;

public class LoginAttemptsService : ILoginAttemptsService
{
    private readonly DataContext _context;

    public LoginAttemptsService(DataContext context)
    {
        _context = context;
    }

    public async Task LogLoginAttempt(Guid accountId, bool isSuccessful, string ipAddress)
    {
        await _context.LoginAttempts.AddAsync(new LoginAttempt
        {
            AccountId = accountId,
            Time = new DateTime(),
            IpAddress = ipAddress,
            IsSuccessful = isSuccessful,
        });
        await _context.SaveChangesAsync();
    }

    public async Task<DateTime?> LastSuccessfulLoginAttemptTime(Guid accountId)
    {
        return (await _context.LoginAttempts.Where(x => x.AccountId == accountId && x.IsSuccessful == true)
            .OrderByDescending(x => x.Time).FirstOrDefaultAsync())
            ?.Time;
    }

    public async Task<DateTime?> LastUnsuccessfulLoginAttemptTime(Guid accountId)
    {
        return (await _context.LoginAttempts.Where(x => x.AccountId == accountId && x.IsSuccessful == false)
            .OrderByDescending(x => x.Time).FirstOrDefaultAsync())
            ?.Time;
    }

    public async Task<int> ThrottleLogInTime(Guid accountId)
    {
        var attempts = await _context.LoginAttempts.Where(x => x.AccountId == accountId).OrderByDescending(x => x.Time)
            .ToListAsync();
        var unsuccessfulAttempts = 0;

        foreach (var attempt in attempts)
        {
            if (attempt.IsSuccessful == false) unsuccessfulAttempts++;
            else break;
        }

        switch (unsuccessfulAttempts)
        {
            case >= 4:
                return 120000;
            case 3:
                return 10000;
            case 2:
                return 5000;
        }

        return 0;
    }

    public async Task<int> ThrottleIpLogIn(Guid accountId, string ipAddress)
    {
        var block = await _context.IpAddressBlocks.FirstOrDefaultAsync(x =>
            x.IpAddress == ipAddress && x.AccountId == accountId);
        if (block != null) return int.MaxValue;

        var attempts = await _context.LoginAttempts.Where(x => x.IpAddress == ipAddress).OrderByDescending(x => x.Time)
            .ToListAsync();
        var unsuccessfulAttempts = 0;

        foreach (var attempt in attempts)
        {
            if (attempt.IsSuccessful == false) unsuccessfulAttempts++;
            else break;
        }

        switch (unsuccessfulAttempts)
        {
            case >= 4:
                await _context.IpAddressBlocks.AddAsync(new IpAddressBlock
                    { IpAddress = ipAddress, AccountId = accountId });
                await _context.SaveChangesAsync();
                return int.MaxValue;
            case 3:
                return 10000;
            case 2:
                return 5000;
        }

        return 0;
    }

    public async Task UnlockIpAddress(Guid accountId, string ipAddress)
    {
        var block = await _context.IpAddressBlocks.FirstOrDefaultAsync(x =>
            x.IpAddress == ipAddress && x.AccountId == accountId);
        if (block != null)
        {
            _context.IpAddressBlocks.Remove(block);
            await _context.SaveChangesAsync();
        }
    }
}