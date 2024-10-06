namespace Api.Validation;

public class Validator(string str)
{
    public string Value { get; } = str;
    public List<string> Errors = [];
    public bool Valid => Errors.Count == 0;

    public Validator Required()
    {
        if (string.IsNullOrEmpty(Value))
        {
            Errors.Add("Field cannot be empty");
        }

        return this;
    }

    public Validator MaxLenght(int lenght)
    {
        if (Value.Length > lenght)
        {
            Errors.Add($"Max lenght is {lenght}");
        }
        return this;
    }

    public Validator MinLenght(int lenght)
    {
        if (Value.Length < lenght)
        {
            Errors.Add($"Min lenght is {lenght}");
        }
        return this;
    }
}