namespace Internship.Data
{
    public class EmailOptions
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FromAddress { get; set; } = default!;
    }
}
