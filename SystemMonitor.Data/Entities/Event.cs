using Microsoft.AspNetCore.Identity;

namespace SystemMonitor.Data.Entities;

public class Event
{
    public int Id { get; set; }

    public required string Resource { get; set; }

    public required DateTime LoggedAt { get; set; }

    public required string Message { get; set; }

    public string? UserAcknowledgedId { get; set; }

    public IdentityUser? UserAcknowledged { get; set; }

    public DateTime? AcknowledgedAt { get; set; }

    public ICollection<string> UsersNotifiedIds { get; set; } = [];

    public ICollection<IdentityUser> UsersNotified { get; set; } = [];
}
