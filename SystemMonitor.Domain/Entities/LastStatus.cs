namespace SystemMonitor.Domain.Entities;

public class LastStatus
{
    public int Id { get; set; }

    public required string Resource { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public DateTime LastNotifiedAt { get; set; }

    public required bool IsOkay { get; set; }

    public required string Message { get; set; }
}
