using AutoMapper;
using Internship.Data;
using static Internship.Data.Constants;
using Internship.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using Hangfire;
using Internship.Mail;
using Internship.LinqExtensions;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using BCrypt.Net;

namespace Internship.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ExportJsonFileHelper _exportJsonFileHelper;
        private readonly WelcomeEmailHelper _welcomeEmailHelper;
        public UserController(Context context, IMapper mapper, ExportJsonFileHelper exportJsonFileHelper, WelcomeEmailHelper welcomeEmailHelper)
        {
            _context = context;
            _mapper = mapper;
            _exportJsonFileHelper = exportJsonFileHelper;
            _welcomeEmailHelper = welcomeEmailHelper;
        }

        /// <summary>
        /// Returns a list of users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<UserNameEmailDTO>>> GetAll()
        {
            IList<User> users = await _context.Users.ToListAsync();

            if (!users.Any())
            {
                return NoContent();
            }

            IList<UserNameEmailDTO> userDTOs = _mapper.Map<IList<User>, IList<UserNameEmailDTO>>(users);

            return Ok(userDTOs);
        }

        /// <summary>
        /// Returns username by user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDataDTO>> GetById(int id)
        {
            User? user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            UserDataDTO userDTO = _mapper.Map<User, UserDataDTO>(user);

            return Ok(userDTO);
        }

        /// <summary>
        /// Updates user object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = AdminRole + ", " + DefaultRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDataDTO>> Update(int id, [FromBody] UserCreateUpdateDTO userDto)
        {
            bool doesUserExist = await _context.Users.AnyAsync(x => x.Username == userDto.Username && x.Id != id);

            if (doesUserExist)
            {
                return BadRequest();
            }

            User? user = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            _mapper.Map(userDto, user);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            Role role = await GetRoleByIdOrGetDefault(userDto.RoleId);
            if(user.RoleId != role.Id)
            {
                user.RoleId = role.Id;
            }

            await _context.SaveChangesAsync();

            UserDataDTO userDataDTO = _mapper.Map<User, UserDataDTO>(user);
            return Ok(userDataDTO);
        }

        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NewUserDTO>> Create([FromBody] UserCreateUpdateDTO userDTO)
        {
            // check if same username exists
            bool doesUserExist = await _context.Users.AnyAsync(x => x.Username == userDTO.Username);

            if (doesUserExist)
            {
                return BadRequest();
            }

            User user = _mapper.Map<UserCreateUpdateDTO, User>(userDTO);
            if (user == null)
            {
                return BadRequest();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Role = await GetRoleByIdOrGetDefault(userDTO.RoleId);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            BackgroundJob.Schedule(() => _welcomeEmailHelper.SendWelcomeEmail(user.Email),TimeSpan.FromMinutes(5));

            NewUserDTO newUserDTO = _mapper.Map<User, NewUserDTO>(user);
            return Ok(newUserDTO);
        }

        /// <summary>
        /// Deletes user object.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<Role> GetRoleByIdOrGetDefault(int roleId)
        {
            Role? role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
            if (role == null)
            {
                //if none of the roles are set, set default one
                role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == DefaultRole);
            }
            return role;
        }

        /// <summary>
        /// Makes json file with list of users.
        /// </summary>
        /// <param name="onlyUsersWithAssignee"></param>
        [HttpPost("json")]
        [Authorize(Roles = AdminRole)]
        public void ExportUserData([FromQuery] bool onlyUsersWithAssignee)
        {
            BackgroundJob.Enqueue(() => _exportJsonFileHelper.CreateJsonFileInBackgroundAsync(onlyUsersWithAssignee));
        }

    }
}
