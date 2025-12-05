namespace SystemMonitor.Domain.Entities;

public class Event
{
    public int Id { get; set; }

    public required string Resource { get; set; }

    public required DateTime LoggedAt { get; set; }

    public required string Status { get; set; }
}
