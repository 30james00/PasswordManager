using Domain;

namespace PasswordManager.Application.Security.Token
{
    public interface ITokenService
    {
        string CreateToken(Account account);
        public RefreshToken GenerateRefreshToken();
    }
}