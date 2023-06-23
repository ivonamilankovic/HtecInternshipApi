using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Internship.Models
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; } = default!;
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [ForeignKey("User")]
        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }
    }
}
