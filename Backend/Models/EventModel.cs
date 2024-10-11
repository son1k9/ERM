using Microsoft.Data.Sqlite;

namespace Models;

public class EventModel(ISqliteConnectionFactory factory)
{
    readonly ISqliteConnectionFactory factory = factory;

    private static Event ScanEvent(SqliteDataReader reader)
    {
        return new Event
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Description = reader.GetString(2),
            Date = reader.GetDateTime(3),
            Place = reader.GetString(4),
            Canceled = reader.GetBoolean(5),
            OraganizerId = reader.GetInt32(6)
        };
    }

    public List<Event> GetEventsForUser(int userId)
    {
        var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"SELECT name, description, date, place, canceled, organizer_id
                     FROM user_event ev INNER JOIN event e ON ev.event_id = e.id
                     WHERE ev.user_id = $id";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", userId);

        using var reader = command.ExecuteReader();
        var events = new List<Event>();
        while (reader.Read())
        {
            events.Add(ScanEvent(reader));
        }
        return events;
    }

    public int Insert(Event _event)
    {
        using var connection = factory.GetConnection();
        connection.OpenAsync();

        var stmt = @"INSERT INTO event (name, description, date, place, canceled, organizer_id) 
                     VALUES ($name, $description, $date, $place, $canceled, $organizer_id);
                     SELECT last_insert_rowid();";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$name", _event.Name);
        command.Parameters.AddWithValue("$description", _event.Description);
        command.Parameters.AddWithValue("$date", _event.Date);
        command.Parameters.AddWithValue("$place", _event.Place);
        command.Parameters.AddWithValue("$canceled", _event.Canceled);
        command.Parameters.AddWithValue("$organizer_id", _event.OraganizerId);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool Update(Event _event)
    {
        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"UPDATE event
                     SET 
                        name = $name,
                        description = $description,
                        date = $date,
                        place = $place,
                        canceled = $canceled,
                        organizer_id = $organizer_id
                     WHERE id = $id;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", _event.Id);
        command.Parameters.AddWithValue("$name", _event.Name);
        command.Parameters.AddWithValue("$description", _event.Description);
        command.Parameters.AddWithValue("$date", _event.Date);
        command.Parameters.AddWithValue("$place", _event.Place);
        command.Parameters.AddWithValue("$canceled", _event.Canceled);
        command.Parameters.AddWithValue("$organizer_id", _event.OraganizerId);

        return command.ExecuteNonQuery() > 0;
    }

    public Event? Get(int id)
    {
        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"SELECT id, name, description, date, place, canceled, organizer_id 
                     FROM event 
                     WHERE id = $id;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return ScanEvent(reader);
        }

        return null;
    }

    public bool Delete(int id)
    {
        using var connection = factory.GetConnection();
        connection.Open();

        var stmt = @"DELETE FROM event
                    WHERE id = $id;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", id);

        return command.ExecuteNonQuery() > 0;
    }
}
