namespace PasswordManager.Application.LoginAttempts;

public interface ILoginAttemptsService
{
    Task LogLoginAttempt(bool isSuccessful, string ipAddress);
    DateTime? LastSuccessfulLoginAttemptTime();
    DateTime? LastUnsuccessfulLoginAttemptTime();
    void ThrottleLogIn();
    void ResetLocks();
}