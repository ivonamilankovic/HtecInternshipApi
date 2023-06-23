using Internship.Models;

namespace Internship.Jwt
{
    public interface IJwtHelper
    {
        public string GenerateToken(User user);
        public int ValidateToken(string token);
    }
}
