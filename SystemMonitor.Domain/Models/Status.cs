namespace SystemMonitor.Domain.Models;

public class Status
{
    public int Id { get; set; }

    public required string Condition { get; set; }

    public required string Value { get; set; }

    public required DateTime Timestamp { get; set; }
}
