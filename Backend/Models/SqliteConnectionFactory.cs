using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;
public class SqliteConnectionFactory(string dbPath) : ISqliteConnectionFactory
{
    readonly string dbPath = dbPath;

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection($"Data Source={dbPath}");
    }
}
