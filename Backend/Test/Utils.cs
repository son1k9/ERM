using Models;

namespace Test;

public class Utils
{
    public static string DbFileName = "Db\\events.db";

    public static ISqliteConnectionFactory GetConnectionFactory()
    {
        var db = Directory.CreateTempSubdirectory().FullName + "\\events.db";
        File.Copy(DbFileName, db);
        File.Copy(DbFileName, db + "-shm");
        File.Copy(DbFileName, db + "-wal");
        return new SqliteConnectionFactory(db);
    }
}
