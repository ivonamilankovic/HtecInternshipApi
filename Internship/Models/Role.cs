using System.ComponentModel.DataAnnotations;

namespace Internship.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = default!;

        public ICollection<User>? Users { get; set; }
    }
}
