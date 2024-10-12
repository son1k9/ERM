namespace Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public string Place { get; set; } = string.Empty;
    public bool Canceled { get; set; }
    public int OrganizerId { get; set; }
}