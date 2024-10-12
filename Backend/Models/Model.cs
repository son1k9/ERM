namespace Models;

public class Model(ISqliteConnectionFactory factory)
{
    public UserModel Users { get; } = new UserModel(factory);
    public EventModel Events { get; } = new EventModel(factory);
    public TokenModel Tokens { get; } = new TokenModel(factory);
}

public class Result<T>
{
    public T? Value { get; }
    public string? Error { get; }

    public Result(T value)
    {
        Value = value;
    }

    public Result(string err)
    {
        Error = err;
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(string err)
    {
        return new Result<T>(err);
    }
}