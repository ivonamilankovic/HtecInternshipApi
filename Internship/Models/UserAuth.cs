using System.ComponentModel.DataAnnotations;

namespace Internship.Models
{
    public class UserAuth
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
