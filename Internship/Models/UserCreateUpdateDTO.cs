using System.ComponentModel.DataAnnotations;

namespace Internship.Models
{
    public class UserCreateUpdateDTO
    {
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        [Required]
        public int RoleId { get; set; } = default!;
    }
}
