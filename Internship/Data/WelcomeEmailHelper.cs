using Internship.Mail;

namespace Internship.Data
{
    public class WelcomeEmailHelper
    {
        private IMailer _mailer;
        public WelcomeEmailHelper(IMailer mailer)
        {
            _mailer = mailer;
        }

        public async Task SendWelcomeEmail(string email)
        {
            await _mailer.SendMailAsync(email, "Welcome", "Welcome, hope you are enjoying our app.");
        }

    }
}
