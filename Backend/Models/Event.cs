namespace Models;

public class Event {
    public int Id;
    public string Name = string.Empty;
    public string Description = string.Empty;
    public DateTime Date = DateTime.Now;
    public string Place = string.Empty;
    public bool Canceled;
    public int OraganizerId;
}