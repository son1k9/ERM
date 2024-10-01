using Microsoft.Data.Sqlite;

namespace Models;

public interface ISqliteConnectionFactory
{
    public SqliteConnection GetConnection();
}
