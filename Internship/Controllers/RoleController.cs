using AutoMapper;
using Internship.Data;
using static Internship.Data.Constants;
using Internship.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;

namespace Internship.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public RoleController(Context context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a list of roles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<NewRoleDTO>>> GetAll()
        {
            IList<Role> roles = await _context.Roles.ToListAsync();

            if(!roles.Any())
            {
                return NoContent();
            }

            IList<NewRoleDTO> result = _mapper.Map<IList<Role>, IList<NewRoleDTO>>(roles);

            return Ok(result);
        }

        /// <summary>
        /// Returns role by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDTO>> GetById(int id)
        {
            Role? role = await _context.Roles.SingleOrDefaultAsync(x => x.Id == id);
            
            if(role == null)
            {
                return NotFound();
            }

            RoleDTO roleDTO = _mapper.Map<Role, RoleDTO>(role);
            return Ok(roleDTO);
        }

        /// <summary>
        /// Creates new role.
        /// </summary>
        /// <param name="newRoleDto"></param>
        /// <returns></returns>
        [HttpPost] 
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NewRoleDTO>> Create([FromBody] RoleDTO newRoleDto)
        {
            if(newRoleDto == null)
            {
                return BadRequest();
            }

            bool doesRoleExist = await _context.Roles.AnyAsync(x => x.Name == newRoleDto.RoleName);
            if(doesRoleExist) 
            {
                return BadRequest();
            }

            Role role = _mapper.Map<RoleDTO, Role>(newRoleDto);
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            NewRoleDTO result = _mapper.Map<Role,NewRoleDTO>(role);
            return Ok(result);
        }

        /// <summary>
        /// Updates role.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateRoleDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDTO>> Update(int id, [FromBody] RoleDTO updateRoleDto)
        {
            bool doesSameRoleExist = await _context.Roles.AnyAsync(x => x.Name == updateRoleDto.RoleName && x.Id != id);
            
            if(doesSameRoleExist)
            {
                return BadRequest();
            }

            Role? existingRole = await _context.Roles.SingleOrDefaultAsync(x => x.Id == id);

            if(existingRole == null)
            {
                return NotFound();
            }
            if(existingRole.Name == DefaultRole)
            {
                return BadRequest(); //can't update default role
            }

            Role role = _mapper.Map<RoleDTO, Role>(updateRoleDto, existingRole);
            await _context.SaveChangesAsync();
            return Ok(updateRoleDto);

        }

        /// <summary>
        /// Deletes role.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            Role? role = await _context.Roles.Include(r => r.Users).SingleOrDefaultAsync(x => x.Id == id);
            
            if(role == null) 
            {
                return NotFound();
            }

            if(role.Name == DefaultRole)
            {
                //can't delete default role
                return BadRequest();
            }
            else
            {
                Role standardRole = await _context.Roles.SingleOrDefaultAsync(x => x.Name == DefaultRole);
                foreach(User u in role.Users)
                {
                    u.Role = standardRole;
                }
            }
            
            _context.Roles.Remove(role);    
            await _context.SaveChangesAsync();
            return Ok();

        }

    }
}
