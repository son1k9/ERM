using Microsoft.Data.Sqlite;
using System.Buffers.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Models;

public class TokenModel(ISqliteConnectionFactory factory)
{
    readonly ISqliteConnectionFactory factory = factory;

    public bool Insert(Token token)
    {
        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"INSERT INTO auth_token(hash, expiry, user_id)
                     VALUES ($hash, $expiry, $user_id);
                     SELECT last_insert_rowid();";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$hash", token.Hash);
        command.Parameters.AddWithValue("$expiry", token.Expiry);
        command.Parameters.AddWithValue("$user_id", token.UserId);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public Token? New(int userId, TimeSpan duration)
    {
        var token = Token.GenerateToken(userId, duration);
        if (Insert(token))
        {
            return token;
        }
        else
        {
            return null;
        }
    }
}