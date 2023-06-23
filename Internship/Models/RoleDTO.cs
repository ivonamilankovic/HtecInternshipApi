using System.ComponentModel.DataAnnotations;

namespace Internship.Models
{
    public class RoleDTO
    {
        [Required]
        public string RoleName { get; set; } = default!;
    }
}
