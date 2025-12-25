namespace SystemMonitor.Domain.Entities;

public class LastStatus
{
    public int Id { get; set; }

    public required string Resource { get; set; }

    public DateTime LoggedAt { get; set; }

    public DateTime LastEmailSendAt { get; set; }

    public required bool IsOkay { get; set; }

    public required string Status { get; set; }
}
