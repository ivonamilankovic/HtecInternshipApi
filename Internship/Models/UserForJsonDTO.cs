namespace Internship.Models
{
    public class UserForJsonDTO
    {
        public int Id { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public RoleForJsonDTO Role { get; set; }
        public AssigneeForJsonDTO? Assignee { get; set; }
    }
}
