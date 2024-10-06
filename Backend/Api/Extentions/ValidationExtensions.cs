using Api.Validation;
using Models;
using System.Net.Mail;

namespace Api.Extentions;

public static class UserExtension
{
    public static Dictionary<string, string[]> Validate(this User user)
    {
        var errors = new Dictionary<string, string[]>();

        var emailValidator = new Validator(user.Email).Required();
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

        var loginValidator = new Validator(user.Login).Required().MinLenght(2).MaxLenght(255);
        if (!loginValidator.Valid)
        {
            errors.Add("login", [.. loginValidator.Errors]);
        }

        var pnValidator = new Validator(user.Phone).Required();
        if (!pnValidator.Valid)
        {
            errors.Add("phone", [.. pnValidator.Errors]);
        }

        var passwordValidator = new Validator(user.Password).Required().MinLenght(10).MaxLenght(255);
        if (!passwordValidator.Valid)
        {
            errors.Add("password", [.. passwordValidator.Errors]);
        }

        return errors;
    }
}