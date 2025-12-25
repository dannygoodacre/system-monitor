using SystemMonitor.Domain.Entities;

namespace SystemMonitor.Application.Abstractions.Data.Repositories;

public interface ILastStatusRepository
{
    void Add(LastStatus entity);

    Task<LastStatus?> GetAsync(string resource, CancellationToken cancellationToken = default);
}
