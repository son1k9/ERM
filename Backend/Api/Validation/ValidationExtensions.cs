using Api.Controllers;
using Models;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Api.Validation;

public static partial class ValidationExtensions
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

        var loginValidator = new StringValidator(user.Login)
        .Required().MinLenght(2).MaxLenght(255)
        .Mathes(LoginRegex(), "Login can contaion only latin letters, digits and '_'");
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

        var nameValidator = new StringValidator(_event.Name).Required().MinLenght(2).MaxLenght(255);
        if (!nameValidator.Valid)
        {
            errors.Add("name", [.. nameValidator.Errors]);
        }

        var descriptionValidator = new StringValidator(_event.Description).Required().MaxLenght(8192);
        if (!descriptionValidator.Valid)
        {
            errors.Add("description", [.. descriptionValidator.Errors]);
        }

        if (_event.Date < DateTime.Now)
        {
            errors.Add("date", ["Date can not be in the past"]);
        }

        var placeValidator = new StringValidator(_event.Place).Required().MaxLenght(512);
        if (!placeValidator.Valid)
        {
            errors.Add("place", [.. placeValidator.Errors]);
        }

        return errors;
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_]+$")]
    private static partial Regex LoginRegex();
}