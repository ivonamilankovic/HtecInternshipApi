﻿namespace Internship.Models
{
    public class AssigneeForJsonDTO
    {
        public int Id { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public RoleForJsonDTO Role { get; set; }
    }
}
