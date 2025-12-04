using System.Diagnostics.CodeAnalysis;

namespace SystemMonitor.Domain.Models;

public sealed record ResourceStatus
{
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsOkay { get; init; } = true;

    public string? ErrorMessage { get; init; }
}
