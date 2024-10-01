namespace Models;

public class Models(ISqliteConnectionFactory factory)
{
    public UserModel Users { get; } = new UserModel(factory);
    public EventModel Events { get; } = new EventModel(factory);
}
