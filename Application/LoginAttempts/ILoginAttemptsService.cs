namespace PasswordManager.Application.LoginAttempts;

public interface ILoginAttemptsService
{
    void LogLoginAttempt(bool isSuccessful, string ipAddress);
    DateTime LastSuccessfulLoginAttemptTime();
    DateTime LastUnsuccessfulLoginAttemptTime();
    void ThrottleLogIn();
    void ResetLocks();
}