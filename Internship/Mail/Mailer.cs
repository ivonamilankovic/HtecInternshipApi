using Internship.Data;
using Internship.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Internship.Mail
{
    public class Mailer : IMailer
    {
        private readonly EmailOptions _options;
        private SmtpClient client;
        public Mailer(IOptions<EmailOptions> options) 
        {
            _options = options.Value;

            client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(_options.Username, _options.Password),
                EnableSsl = true
            };
        }

        public async Task SendMailAsync(string ToAddress, string title, string msgText)
        {
            MailMessage message = new MailMessage(_options.FromAddress, ToAddress, title, msgText);
            await client.SendMailAsync(message);
            Console.WriteLine("Mail sent to " + ToAddress);
        }

        public async Task SendMailWithAttachmentAsync(string ToAddress, string title, string msgText, string fileName) 
        {
            MailMessage message = new MailMessage(_options.FromAddress, ToAddress, title, msgText);
            string path = Environment.CurrentDirectory + "\\";
            message.Attachments.Add(new Attachment(path + fileName));
            await client.SendMailAsync(message);
            Console.WriteLine("Mail sent to " + ToAddress);
        }
    }
}
