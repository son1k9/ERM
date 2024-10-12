using System.Text.RegularExpressions;

namespace Api.Validation;

public class StringValidator(string str)
{
    public string? Value { get; } = str;
    public List<string> Errors = [];
    public bool Valid => Errors.Count == 0;

    public StringValidator Required()
    {
        if (string.IsNullOrEmpty(Value))
        {
            Errors.Add("Field cannot be empty");
        }

        return this;
    }

    public StringValidator MaxLenght(int lenght)
    {
        if (Value?.Length > lenght)
        {
            Errors.Add($"Max lenght is {lenght}");
        }
        return this;
    }

    public StringValidator MinLenght(int lenght)
    {
        if (Value?.Length < lenght)
        {
            Errors.Add($"Min lenght is {lenght}");
        }
        return this;
    }

    public StringValidator Mathes(Regex regex, string message)
    {
        if (Value != null)
        {
            if (!regex.IsMatch(Value))
            {
                Errors.Add(message);
            };
        }
        return this;
    }
}