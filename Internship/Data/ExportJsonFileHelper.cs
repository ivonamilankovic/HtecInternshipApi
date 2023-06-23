using AutoMapper;
using Internship.LinqExtensions;
using Internship.Mail;
using Internship.Models;
using Microsoft.EntityFrameworkCore;

namespace Internship.Data
{
    public class ExportJsonFileHelper
    {
        private Context _context;
        private IMailer _mailer;
        private IMapper _mapper;
        public ExportJsonFileHelper(Context context, IMailer mailer, IMapper mapper)
        {
            _context = context;
            _mailer = mailer;
            _mapper = mapper;
        }

        public async Task CreateJsonFileInBackgroundAsync(bool onlyUsersWithAssignee)
        {
            List<User> users = await _context.Users
                                    .Include(u => u.Assignee)
                                    .Include(u => u.Role)
                                    .WhereIf(onlyUsersWithAssignee, u => u.Assignee != null)
                                    .ToListAsync();
            List<UserForJsonDTO> usersForJson = _mapper.Map<List<User>, List<UserForJsonDTO>>(users);
            string fileName = "users.json";
            JsonFileUtils.MakeFile(usersForJson, fileName);
            await _mailer.SendMailWithAttachmentAsync("admin@example.com", "Notification",
                "Json file is downloaded (check attachments of this email).", fileName);
        }


    }
}
