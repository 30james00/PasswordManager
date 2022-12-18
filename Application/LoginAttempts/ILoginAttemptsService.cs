namespace PasswordManager.Application.LoginAttempts;

public interface ILoginAttemptsService
{
    Task LogLoginAttempt(Guid accountId, bool isSuccessful, string ipAddress);
    Task<DateTime?> LastSuccessfulLoginAttemptTime(Guid accountId);
    Task<DateTime?> LastUnsuccessfulLoginAttemptTime(Guid accountId);
    Task<int> ThrottleLogInTime(Guid accountId);
    Task<int> ThrottleIpLogIn(Guid accountId, string ipAddress);
    Task UnlockIpAddress(Guid accountId, string ipAddress);
}