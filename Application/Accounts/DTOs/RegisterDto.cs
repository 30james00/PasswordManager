using System.ComponentModel.DataAnnotations;

namespace PasswordManager.Application.Accounts.DTOs
{
    public class RegisterDto
    {
        [Required] public string Login { get; set; } = null!;

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
        public string Password { get; set; } = null!;

        [Required] public bool IsPasswordKeptAsHash { get; set; }
    }
}