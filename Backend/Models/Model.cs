namespace Models;

public class Model(ISqliteConnectionFactory factory)
{
    public UserModel Users { get; } = new UserModel(factory);
    public EventModel Events { get; } = new EventModel(factory);
}
