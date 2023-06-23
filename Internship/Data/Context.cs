using Microsoft.EntityFrameworkCore;
using Internship.Models;

namespace Internship.Data
{
    public class Context : DbContext
    {
        public Context() { }

        public Context(DbContextOptions<Context> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
    }
}
