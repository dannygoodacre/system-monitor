using SystemMonitor.Data.Entities;

namespace SystemMonitor.Data.Extensions;

internal static class EventExtensions
{
    public static SystemMonitor.Domain.Models.Event ToModel(this Event value)
    {
        return new Domain.Models.Event
        {
            LoggedAt = value.LoggedAt,
            Message = value.Message,
            UserAcknowledgedId = value.UserAcknowledgedId,
            AcknowledgedAt = value.AcknowledgedAt,
            UsersNotifiedIds = value.UsersNotifiedIds,
            Resource = value.Resource
        };
    }

    public static Event ToEntity(this SystemMonitor.Domain.Models.Event value)
        => new()
        {
            LoggedAt = value.LoggedAt,
            Message = value.Message,
            UserAcknowledgedId = value.UserAcknowledgedId,
            AcknowledgedAt = value.AcknowledgedAt,
            Resource = value.Resource
        };
}
