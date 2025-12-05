
using SystemMonitor.Domain.Entities;

namespace SystemMonitor.Application.Abstractions.Data.Repositories;

public interface IEventRepository
{
    void Add(Event @event);

    Task<Event?> GetLatestAsync(string resource, CancellationToken cancellationToken = default);
}
