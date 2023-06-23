namespace Internship.Mail
{
    public class FakeMailer : IMailer
    {
        public bool isSent { get; set; }
        public FakeMailer() 
        {
            isSent = false;
        }
        public async Task SendMailAsync(string ToAddress, string title, string msgText) 
        {
            isSent = true;
        }
    }
}
