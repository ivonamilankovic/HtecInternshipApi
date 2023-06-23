using AutoMapper;
using Internship.Data;
using Internship.LinqExtensions;
using Internship.Models;
using Internship.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using Internship.Migrations;
using Microsoft.AspNetCore.Authorization;
using static Internship.Data.Constants;

namespace Internship.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly IMailer _mailer;
        public AssignmentController(Context context, IMapper mapper, IMailer mailer) 
        {
            _context = context;
            _mapper = mapper;
            _mailer = mailer;
        }

        /// <summary>
        /// Returns all mentor-assignee.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<MentorAssigneeInfoDTO>>> GetAll([FromQuery]bool mentorsWithAssignee)
        {
            IList<User> users = await _context.Users.Include(u => u.Assignee)
                                      .WhereIf(mentorsWithAssignee, u => u.Assignee != null)
                                      .ToListAsync();
            
            if(!users.Any())
            {
                return NoContent();
            }

            IList<MentorAssigneeInfoDTO> mentorAssigneeInfos = _mapper.Map<IList<User>, IList<MentorAssigneeInfoDTO>>(users);

            return Ok(mentorAssigneeInfos);
        }

        /// <summary>
        /// Returns mentor-assignee by assignee id.
        /// </summary>
        /// <param name="assigneeId"></param>
        /// <returns></returns>
        [HttpGet("Assignee/{assigneeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MentorAssigneeInfoDTO>> GetByAssigneeId(int assigneeId)
        {
            bool doesUserExist = await _context.Users.AnyAsync(x => x.Id == assigneeId);
            if(!doesUserExist)
            {
                return NotFound();
            }

            User? mentor = await _context.Users.Include(u => u.Assignee).FirstOrDefaultAsync(x => x.AssigneeId == assigneeId);
            
            if(mentor == null)
            {
                return BadRequest("This user is not assignee.");
            }

            MentorAssigneeInfoDTO mentorAssigneeInfo = _mapper.Map<User, MentorAssigneeInfoDTO>(mentor);
            return Ok(mentorAssigneeInfo);

        }

        /// <summary>
        /// Returns mentor-assignee by mentor id.
        /// </summary>
        /// <param name="mentorId"></param>
        /// <returns></returns>
        [HttpGet("Mentor/{mentorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MentorAssigneeInfoDTO>> GetByMentorId(int mentorId)
        {
            User? mentor = await _context.Users.Include(u => u.Assignee).FirstOrDefaultAsync(x => x.Id == mentorId);

            if (mentor == null)
            {
                return NotFound();
            }

            MentorAssigneeInfoDTO mentorAssigneeInfo = _mapper.Map<User, MentorAssigneeInfoDTO>(mentor);
            return Ok(mentorAssigneeInfo);

        }

        /// <summary>
        /// Sets assignee to mentor.
        /// </summary>
        /// <param name="mentorAssigneeDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = AdminRole + ", " + DefaultRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MentorAssigneeInfoDTO>> SetAssigneeToMentor([FromBody]MentorAssigneeDTO mentorAssigneeDTO)
        {
            User? mentor = await FindUserAsync(mentorAssigneeDTO.MentorId); 
            User? assignee = await FindUserAsync(mentorAssigneeDTO.AssigneeId);
            if (assignee == null || mentor == null)
            {
                return NotFound();
            }

            bool result = await CheckIfAssignmentIsPossibleAsync(mentorAssigneeDTO.MentorId, mentorAssigneeDTO.AssigneeId);
            if (!result)
            {
                return BadRequest();
            }

            //mentor should not have assignee
            if(mentor.AssigneeId != null)
            {
                return BadRequest();
            }

            mentor.Assignee = assignee;
            await _context.SaveChangesAsync();

            await SendNewAssignmentEmailAsync(mentor.Username, mentor.Email, assignee.Username, assignee.Email);

            MentorAssigneeInfoDTO info = _mapper.Map<User, MentorAssigneeInfoDTO>(mentor); 
            return Ok(info);

        }

        /// <summary>
        /// Changes assignee to mentor by putting new one.
        /// </summary>
        /// <param name="mentorId"></param>
        /// <param name="newAssigneeId"></param>
        /// <returns></returns>
        [HttpPut("{mentorId}/{newAssigneeId}")]
        [Authorize(Roles = AdminRole + ", " + DefaultRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ChangeAssignee(int mentorId, int newAssigneeId)
        {
            User? mentor = await FindUserAsync(mentorId);
            User? newAssignee = await FindUserAsync(newAssigneeId);
            if (newAssignee == null || mentor == null)
            {
                return NotFound();
            }
            
            bool result = await CheckIfAssignmentIsPossibleAsync(mentorId, newAssigneeId);
            if(!result)
            {
                return BadRequest();
            }

            mentor.Assignee = newAssignee;
            await _context.SaveChangesAsync();

            await SendNewAssignmentEmailAsync(mentor.Username, mentor.Email, newAssignee.Username, newAssignee.Email);

            MentorAssigneeInfoDTO mentorAssigneeInfo = _mapper.Map<User,MentorAssigneeInfoDTO>(mentor);
            return Ok(mentorAssigneeInfo);

        }

        /// <summary>
        /// Removes assignee from mentor.
        /// </summary>
        /// <param name="assigneeId"></param>
        /// <returns></returns>
        [HttpDelete("Assignee/{assigneeId}")]
        [Authorize(Roles = AdminRole + ", " + DefaultRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveAssigneeFromMentor(int assigneeId)
        {
            User? mentor = await _context.Users.Include(u=>u.Assignee)
                                .FirstOrDefaultAsync(x => x.AssigneeId == assigneeId);
            if(mentor == null)
            {
                return NotFound();
            }

            await SendBrokenAssignmnetEmailAsync(mentor.Username, mentor.Email, mentor.Assignee.Username, mentor.Assignee.Email);

            mentor.Assignee = null;
            mentor.AssigneeId = null;
            await _context.SaveChangesAsync();
            return Ok();

        }

        /// <summary>
        /// Removes mentor from assignee.
        /// </summary>
        /// <param name="mentorId"></param>
        /// <returns></returns>
        [HttpDelete("Mentor/{mentorId}")]
        [Authorize(Roles = AdminRole + ", " + DefaultRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveMentorFromAssignee(int mentorId)
        {
            User? mentor = await _context.Users.Include(u => u.Assignee)
                                .FirstOrDefaultAsync(x => x.Id == mentorId);
            if (mentor == null)
            {
                return NotFound();
            }

            await SendBrokenAssignmnetEmailAsync(mentor.Username, mentor.Email, mentor.Assignee.Username, mentor.Assignee.Email);

            mentor.Assignee = null;
            mentor.AssigneeId = null;
            await _context.SaveChangesAsync();
            return Ok();

        }

        public async Task SendNewAssignmentEmailAsync(string mentorName, string mentorEmail, string assigneeName, string assigneeEmail)
        {
            string title = "New assignment!";
            string mentorMsg = "Hi! Your new assignee is " + assigneeName + ".";
            await _mailer.SendMailAsync(mentorEmail, title, mentorMsg);
            string assigneeMsg = "Hi! Your new mentor is " + mentorName + ".";
            await _mailer.SendMailAsync(assigneeEmail, title, assigneeMsg);
        }
        public async Task SendBrokenAssignmnetEmailAsync(string mentorName, string mentorEmail, string assigneeName, string assigneeEmail)
        {
            string title = "Assignment is no longer active";
            string mentorMsg = "Hi!" + assigneeName + " is no longer your assignee.";
            await _mailer.SendMailAsync(mentorEmail, title, mentorMsg);
            string assigneeMsg = "Hi! " + mentorName + "is no longer your mentor.";
            await _mailer.SendMailAsync(assigneeEmail, title, assigneeMsg);
        }

        private async Task<bool> DoesAssigneeHaveMentorAsync(int assigneeId)
        {
            return await _context.Users.AnyAsync(x => x.AssigneeId == assigneeId);
        }

        private async Task<User?> FindUserAsync(int id)
        {
            return await _context.Users.Include(u => u.Assignee).FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task<bool> CheckIfAssignmentIsPossibleAsync(int mentorId, int assigneeId)
        {
            if (mentorId == assigneeId)
            {
                return false;
            }

            User? mentor = await FindUserAsync(mentorId);
            User? newAssignee = await FindUserAsync(assigneeId);
            
            if (mentor.AssigneeId == assigneeId)
            {
                return false;
            }

            if (newAssignee.AssigneeId == mentorId)
            {
                return false;
            }

            bool doesNewAssigneeHaveMentor = await DoesAssigneeHaveMentorAsync(assigneeId);
            if (doesNewAssigneeHaveMentor)
            {
                return false;
            }

            return true;
        }

    }
}
