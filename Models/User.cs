using System.ComponentModel.DataAnnotations;

namespace TeaShop.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string? Username { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(150)]
        public string? Email { get; set; }

        [Required]
        [MinLength(8)]
        public string? Password { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
