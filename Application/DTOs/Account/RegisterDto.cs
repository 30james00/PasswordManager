using System.ComponentModel.DataAnnotations;

namespace PasswordManager.Application.DTOs.Account
{
    public class RegisterDto
    {
        [Required] public string Login { get; set; }

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
        public string Password { get; set; }
    }
}