using System.ComponentModel.DataAnnotations;

namespace TeaShop.Models
{
    public class SignupViewModel : IValidatableObject
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 charachters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
            {
                yield return new ValidationResult("Email must contain '@' and '.'", new[] { nameof(Email) });
            }
        }
    }
} 