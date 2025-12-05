using Microsoft.EntityFrameworkCore;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Domain.Entities;

namespace SystemMonitor.Data.Repositories;

public class EventRepository(ApplicationContext context) : IEventRepository
{
    public void Add(Event @event)
        => context.Events.Add(@event);

    public Task<Event?> GetLatestAsync(string resource, CancellationToken cancellationToken = default)
        => context.Events.FirstOrDefaultAsync(x => x.Resource == resource, cancellationToken);
}
