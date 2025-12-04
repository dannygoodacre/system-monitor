using SystemMonitor.Domain.Models;

namespace SystemMonitor.Application.Abstractions.Data.Repositories;

public interface IEventRepository
{
    void Add(Event @event);

    Task<Event?> GetLatestAsync(string resource, CancellationToken cancellationToken = default);
}
