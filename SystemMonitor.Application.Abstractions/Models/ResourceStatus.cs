using System.Diagnostics.CodeAnalysis;

namespace SystemMonitor.Application.Abstractions.Models;

public class ResourceStatus
{
    [MemberNotNullWhen(true, nameof(Status))]
    public required bool IsOkay { get; init; }

    public required bool ShouldNotify { get; init; }

    public string? Status { get; init; }

    public static ResourceStatus Okay()
        => new()
        {
            IsOkay = true,
            ShouldNotify = false
        };

    public static ResourceStatus NotOkay(string status, bool shouldNotify)
        => new()
        {
            IsOkay = false,
            ShouldNotify = shouldNotify
        };
}
