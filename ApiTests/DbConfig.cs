using Internship.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApiTests
{
    public class DbConfig
    {
        public DbConfig() { }

        public Context SqliteContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<Context>().UseSqlite(connection).Options;
            var context = new Context(options);
            context.Database.EnsureCreated();

            SqliteCommand command = new SqliteCommand();
            using (command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Roles(Id,Name) VALUES (@rid1,@name1),(@rid2,@name2)";
                command.Prepare();
                command.Parameters.AddWithValue("@rid1", 1);
                command.Parameters.AddWithValue("@name1", "admin");
                command.Parameters.AddWithValue("@rid2", 2);
                command.Parameters.AddWithValue("@name2", "Standard User");
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Users(Id,Username,Email,Password,RoleId) "
                                        + "VALUES (@id1,@uname1,@email1,@pswd1,@role1);";
                command.Prepare();
                command.Parameters.AddWithValue("@id1", 1);
                command.Parameters.AddWithValue("@uname1", "user1");
                command.Parameters.AddWithValue("@email1", "test1@mail.com");
                command.Parameters.AddWithValue("@pswd1", "password");
                command.Parameters.AddWithValue("@role1", 1);
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Users(Id,Username,Email,Password,RoleId,AssigneeId) "
                                        + "VALUES (@id2,@uname2,@email2,@pswd2,@role2, @a2),"
                                        + "(@id3,@uname3,@email3,@pswd3,@role3,@a3);";
                command.Prepare();
                command.Parameters.AddWithValue("@id2", 2);
                command.Parameters.AddWithValue("@uname2", "user2");
                command.Parameters.AddWithValue("@email2", "test2@mail.com");
                command.Parameters.AddWithValue("@pswd2", "password");
                command.Parameters.AddWithValue("@role2", 2);
                command.Parameters.AddWithValue("@a2", 3);
                command.Parameters.AddWithValue("@id3", 3);
                command.Parameters.AddWithValue("@uname3", "user3");
                command.Parameters.AddWithValue("@email3", "test3@mail.com");
                command.Parameters.AddWithValue("@pswd3", "password");
                command.Parameters.AddWithValue("@role3", 2);
                command.Parameters.AddWithValue("@a3", 1);
                command.ExecuteNonQuery();
            }

            return context;
        }
    }
}
