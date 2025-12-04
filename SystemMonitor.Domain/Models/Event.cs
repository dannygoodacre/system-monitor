namespace SystemMonitor.Domain.Models;

public class Event
{
    public required string Resource { get; set; }

    public required DateTime LoggedAt { get; init; }

    public required string Message { get; init; }

    public string? UserAcknowledgedId { get; init; }

    public DateTime? AcknowledgedAt { get; init; }

    public ICollection<string> UsersNotifiedIds { get; set; } = [];
}
