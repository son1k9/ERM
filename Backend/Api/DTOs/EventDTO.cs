using Models;

namespace Api.DTOs;

public class EventDTO(Event _event, UserDTO user)
{
    public int Id { get; set; } = _event.Id;
    public string Name { get; set; } = _event.Name;
    public string Description { get; set; } = _event.Description;
    public DateTime Date { get; set; } = _event.Date;
    public string Place { get; set; } = _event.Place;
    public bool Canceled { get; set; } = _event.Canceled;
    public UserDTO Organizer { get; set; } = user;
}