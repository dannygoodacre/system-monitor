using Microsoft.EntityFrameworkCore;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Data.Extensions;
using SystemMonitor.Domain.Models;

namespace SystemMonitor.Data.Repositories;

public class EventRepository(ApplicationContext context) : IEventRepository
{
    public void Add(Event @event)
        => context.Events.Add(@event.ToEntity());

    public async Task<Event?> GetLatestAsync(string resource, CancellationToken cancellationToken = default)
        => (await context.Events.FirstOrDefaultAsync(x => x.Resource == resource, cancellationToken))?.ToModel();
}
