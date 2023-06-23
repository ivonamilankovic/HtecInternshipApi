using Internship.Models;

namespace Internship.Mail
{
    public interface IMailer
    {
        public async Task SendMailAsync(string ToAddress, string title, string msgText) { }
        public async Task SendMailWithAttachmentAsync(string ToAddress, string title, string msgText, string fileName) { }
    }
}
