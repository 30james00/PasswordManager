namespace PasswordManager.Application.Security.Token
{
    public interface IUserAccessor
    {
        string? GetUserId();
        string GetRequestIpAddress();
    }
}