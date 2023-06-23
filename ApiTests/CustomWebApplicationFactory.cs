using Internship.Mail;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        public CustomWebApplicationFactory()
        {
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                       d => d.ServiceType ==
                           typeof(DbContextOptions));

                services.Remove(dbContextDescriptor);

                services.AddDbContext<Internship.Data.Context>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    container.UseSqlite(connection);
                });

                services.AddTransient<IMailer, FakeMailer>();
            });
        }

    }
}
