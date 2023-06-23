using Internship.Mail;

namespace ApiTests
{
    public class MailerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        public MailerTests() { }
        
        [Fact]
        public async Task SendMailTest()
        {
            FakeMailer fakeMailer = new FakeMailer();
            fakeMailer.SendMailAsync("to@address.com", "title", "msg");
            Assert.True(fakeMailer.isSent);
        }

    }
}
