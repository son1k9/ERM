﻿using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;

namespace Models;

public static class UserError
{
    public const string DuplicateEmail = "User with email already exists";
    public const string DuplicateLogin = "User with login already exists";
}

public class UserModel(ISqliteConnectionFactory factory)
{
    readonly ISqliteConnectionFactory factory = factory;

    private static User ScanUser(SqliteDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(0),
            Email = reader.GetString(1),
            Login = reader.GetString(2),
        };
    }

    public User? GetByEmailAndPassword(string email, string password)
    {
        using var connection = factory.GetConnection();
        connection.OpenAsync();

        var stmt = @"SELECT id, email, login, hashed_password FROM user WHERE email = $email;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$email", email);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var user = ScanUser(reader);
            if (User.PasswordMath(password, reader.GetString(3)))
            {
                return user;
            }
        }

        return null;
    }

    public User? GetByLogin(string login)
    {
        using var connection = factory.GetConnection();
        connection.OpenAsync();

        var stmt = @"SELECT id, email, login FROM user WHERE login = $login;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$login", login);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return ScanUser(reader);
        }

        return null;
    }

    public User? GetForToken(string tokenText)
    {
        var tokenHash = Token.GenHash(tokenText);

        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"SELECT u.id, u.email, u.login
                     FROM auth_token t INNER JOIN user u ON t.user_id = u.id
                     WHERE t.hash = $hash AND t.expiry > datetime('now')";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$hash", tokenHash);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            return ScanUser(reader);
        }

        return null;
    }

    public List<User> GetUsersForEvent(int eventId)
    {
        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"SELECT u.id, u.email, u.login 
                    FROM user_event ev INNER JOIN user u ON ev.user_id = u.id  
                    WHERE ev.event_id = $id;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", eventId);

        using var reader = command.ExecuteReader();
        var users = new List<User>();
        while (reader.Read())
        {
            users.Add(ScanUser(reader));
        }
        return users;
    }

    public Result<int> Insert(User user)
    {
        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"INSERT INTO user (email, login, hashed_password) 
                     VALUES ($email, $login, $hashed_password);
                     SELECT last_insert_rowid();";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$login", user.Login);
        var passwordHash = User.HashPassword(user.Password);
        command.Parameters.AddWithValue("$hashed_password", passwordHash);

        try
        {
            return Convert.ToInt32(command.ExecuteScalar());
        }
        catch (SqliteException e)
        {
            if (e.ErrorCode == 19)
            {
                if (e.Message.Contains("user.email"))
                {
                    return UserError.DuplicateEmail;
                }
                if (e.Message.Contains("user.login"))
                {
                    return UserError.DuplicateLogin;
                }
            }
            throw;
        }
    }

    public User? Get(int id)
    {
        using var connection = factory.GetConnection();
        connection.OpenAsync();

        var stmt = @"SELECT id, email, login FROM user WHERE id = $id;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return ScanUser(reader);
        }

        return null;
    }
}