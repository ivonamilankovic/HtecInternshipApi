using System.ComponentModel.DataAnnotations;

namespace Internship.Models
{
    public class UserDataDTO
    {
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        [Required]
        public string RoleName { get; set; } = default!;
    }
}
