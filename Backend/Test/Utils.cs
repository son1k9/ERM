using Models;

namespace Test;

public class Utils
{
    public static string DbFileName = "Db\\events.db";

    public static ISqliteConnectionFactory GetConnectionFactory()
    {
        var db = Path.Combine(Directory.CreateTempSubdirectory().FullName, "events.db");
        File.Copy(DbFileName, db);
        File.Copy(DbFileName + "-shm", db + "-shm");
        File.Copy(DbFileName + "-wal", db + "-wal");
        return new SqliteConnectionFactory(db);
    }

    public static User CreateUser()
    {
        return new User
        {
            Email = "test@test.test",
            Login = "test_login",
            Password = "password"
        };
    }

    public static Event CreateEvent()
    {
        return new Event
        {
            Name = "Test",
            Description = "TestD",
            Date = new DateTime(1984, 6, 9),
            Place = "TestP",
            Canceled = false,
        };
    }

}
