﻿using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Models;

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
            Phone = reader.GetString(3),
        };
    }

    public List<User> GetUsersForEvent(int eventId)
    {
        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"SELECT u.id, u.email, u.login, u.phone
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

    public int Insert(User user)
    {
        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"INSERT INTO user (email, login, phone, hashed_password) 
                     VALUES ($email, $login, $phone, $hashed_password);
                     SELECT last_insert_rowid();";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$login", user.Login);
        command.Parameters.AddWithValue("$phone", user.Phone);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, 12, false);
        command.Parameters.AddWithValue("$hashed_password", passwordHash);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public User? Get(int id)
    {
        using var connection = factory.GetConnection();
        connection.OpenAsync();

        var stmt = @"SELECT id, email, login, phone FROM user WHERE id = $id;";

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