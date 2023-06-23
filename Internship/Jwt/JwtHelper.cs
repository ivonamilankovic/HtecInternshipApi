using Internship.Data;
using Internship.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Internship.Jwt
{
    public class JwtHelper : IJwtHelper
    {
        private readonly SymmetricSecurityKey _securityKey;
        private readonly JwtOptions _jwtOptions;
        private readonly TokenValidationParameters _validationParameters;

        public JwtHelper(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            _validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = _securityKey
            };
        }

        public string GenerateToken(User user)
        {
            SigningCredentials credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
           
            Claim[] claims = new Claim[]
            {
                new(nameof(User.Id), Convert.ToString(user.Id)),
                new(nameof(User.Email), user.Email),
                new(ClaimTypes.Role, user.Role.Name)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                 _jwtOptions.Issuer,
                 _jwtOptions.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtOptions.ExpirationTime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int ValidateToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            ClaimsPrincipal claimsPrincipal = handler.ValidateToken(token, _validationParameters,   
                out SecurityToken validatedToken);
            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
            int userId = int.Parse(jwtToken.Claims.First(x => x.Type == nameof(User.Id)).Value);

            return userId;
        }
    }
}
