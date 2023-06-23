using AutoMapper;
using Internship.Data;
using Internship.Jwt;
using Internship.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Internship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly Context _context;
        private readonly IJwtHelper _jwtHelper;
        private readonly IMapper _mapper;

        public TokenController(Context context, IJwtHelper jwtHelper, IMapper mapper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates Jwt token for authentication.
        /// </summary>
        /// <param name="userAuth"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateToken(UserAuth userAuth)
        {
            User? user = await _context.Users.Include(u => u.Role)
                                .SingleOrDefaultAsync(x => x.Email == userAuth.Email);
            if (user == null)
            {
                return BadRequest();
            }
            if(!BCrypt.Net.BCrypt.Verify(userAuth.Password, user.Password))
            {
                return BadRequest();
            }

            string token = _jwtHelper.GenerateToken(user);
            return Ok(token);
        }

        /// <summary>
        /// Validates Jwt token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ValidateToken(string token)
        {
            int userId = _jwtHelper.ValidateToken(token);

            User? user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return BadRequest();
            }

            UserDataDTO userData = _mapper.Map<User,UserDataDTO>(user);
            return Ok(userData);
        }
    }
}
