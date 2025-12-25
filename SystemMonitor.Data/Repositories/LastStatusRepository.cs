using Microsoft.EntityFrameworkCore;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Domain.Entities;

namespace SystemMonitor.Data.Repositories;

public class LastStatusRepository(ApplicationContext context) : ILastStatusRepository
{
    public void Add(LastStatus entity)
        => context.LastStatuses.Add(entity);

    public Task<LastStatus?> GetAsync(string resource, CancellationToken cancellationToken)
        => context.LastStatuses.FirstOrDefaultAsync(x => x.Resource == resource, cancellationToken);
}
