namespace SystemMonitor.Domain.Models;

public class User
{
    public required string Id { get; set; }

    public required string Email { get; init; }
}
