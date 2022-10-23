using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace PasswordManager.Application.Security.Token
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        /// <summary>
        /// Allows to read user id info from HTTP request
        /// </summary>
        /// <returns>Account Id string</returns>
        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}