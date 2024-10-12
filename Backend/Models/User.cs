namespace Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;

    public string Password = string.Empty;
    public string PasswordHash = string.Empty;

    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12, false);
    }

    public static bool PasswordMath(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}