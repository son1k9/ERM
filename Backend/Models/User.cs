using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Models;

public class User
{
    public int Id;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public static class UserExtension
{
    public static Dictionary<string, string> Validate(this User user)
    {
        var errors = new Dictionary<string, string>();

        bool emailValid = MailAddress.TryCreate(user.Email, out _);
        if (!emailValid)
        {
            
        }
    }
}