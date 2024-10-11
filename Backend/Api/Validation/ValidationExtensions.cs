using Models;
using System.Net.Mail;

namespace Api.Validation;

public static class ValidationExtensions
{
    public static Dictionary<string, string[]> Validate(this User user)
    {
        var errors = new Dictionary<string, string[]>();

        var emailValidator = new StringValidator(user.Email).Required();
        if (!emailValidator.Valid)
        {
            errors.Add("email", [.. emailValidator.Errors]);
        }
        else
        {
            if (!MailAddress.TryCreate(user.Email, out _))
            {
                errors.Add("email", ["Email is incorrect"]);
            }
        }

        var loginValidator = new StringValidator(user.Login).Required().MinLenght(2).MaxLenght(255);
        if (!loginValidator.Valid)
        {
            errors.Add("login", [.. loginValidator.Errors]);
        }

        var passwordValidator = new StringValidator(user.Password).Required().MinLenght(10).MaxLenght(255);
        if (!passwordValidator.Valid)
        {
            errors.Add("password", [.. passwordValidator.Errors]);
        }

        return errors;
    }

    public static Dictionary<string, string[]> Validate(this Event _event)
    {
        var errors = new Dictionary<string, string[]>();
        return errors;
    }
}