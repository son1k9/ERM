namespace Models;

public class UserModel(ISqliteConnectionFactory factory)
{
    readonly ISqliteConnectionFactory factory = factory;

    public async Task Insert(User user)
    {
        using var connection = factory.GetConnection();
        var stmt = @"INSERT INTO user (email, login, phone, hashed_password) 
                     VALUES ($email, $login, $phone, $hashed_password)";
        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$email", user.Id);
        command.Parameters.AddWithValue("$login", user.Login);
        command.Parameters.AddWithValue("$phone", user.Email);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, 12, false);
        command.Parameters.AddWithValue("$hashed_password", passwordHash);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<User?> Get(int id)
    {
        using var connection = factory.GetConnection();
        var stmt = @"SELECT id, email, login, phone FROM user WHERE id = $id";
        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (reader.HasRows)
        {
            var user = new User
            {
                Id = reader.GetInt32(0),
                Email = reader.GetString(1),
                Login = reader.GetString(2),
                Phone = reader.GetString(3)
            };
            return user;
        }

        return null;
    }
}