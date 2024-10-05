namespace Models;

public class EventModel(ISqliteConnectionFactory factory)
{
    readonly ISqliteConnectionFactory factory = factory;

    public async Task<int> Insert(Event _event)
    {
        using var connection = factory.GetConnection();
        await connection.OpenAsync();

        var stmt = @"INSERT INTO event (name, description, date, place, canceled, oraganizer_id) 
                     VALUES ($name, $description, $date, $place, $canceled, $oraganizer_id);
                     SELECT last_insert_rowid();";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$name", _event.Name);
        command.Parameters.AddWithValue("$description", _event.Description);
        command.Parameters.AddWithValue("$date", _event.Date);
        command.Parameters.AddWithValue("$place", _event.Place);
        command.Parameters.AddWithValue("$canceled", _event.Canceled);
        command.Parameters.AddWithValue("$organizer_id", _event.OraganizerId);

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task Update(Event _event)
    {
        using var connection = factory.GetConnection();
        await connection.OpenAsync();

        var stmt = @"UPDATE event
                     WHERE id = $id
                     SET 
                        name = $name,
                        description = $description,
                        date = $date,
                        place = $place,
                        canceled = $canceled,
                        organizer_id = $organizer_id;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$name", _event.Name);
        command.Parameters.AddWithValue("$description", _event.Description);
        command.Parameters.AddWithValue("$date", _event.Date);
        command.Parameters.AddWithValue("$place", _event.Place);
        command.Parameters.AddWithValue("$canceled", _event.Canceled);
        command.Parameters.AddWithValue("$organizer_id", _event.OraganizerId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<Event?> Get(int id)
    {
        using var connection = factory.GetConnection();
        await connection.OpenAsync();

        var stmt = @"SELECT name, description, date, place, canceled, organizer_id 
                     FROM event 
                     WHERE id = $id;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (reader.HasRows)
        {
            var _event = new Event
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Date = reader.GetFieldValue<DateTime>(3),
                Place = reader.GetString(4),
                Canceled = reader.GetBoolean(5),
                OraganizerId = reader.GetInt32(6)
            };
            return _event;
        }

        return null;
    }

    public async Task Delete(int id)
    {
        using var connection = factory.GetConnection();
        await connection.OpenAsync();

        var stmt = @"DELETE FROM event
                    WHERE id = $id;";

        var command = connection.CreateCommand();
        command.CommandText = stmt;
        command.Parameters.AddWithValue("$id", id);

        await command.ExecuteNonQueryAsync();
    }
}
