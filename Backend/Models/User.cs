using System.Net.Mail;

namespace Models;

public class User
{
    public int Id;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}