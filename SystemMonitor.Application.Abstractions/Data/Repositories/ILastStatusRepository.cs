using SystemMonitor.Domain.Entities;

namespace SystemMonitor.Application.Abstractions.Data.Repositories;

public interface ILastStatusRepository
{
    Task<LastStatus?> GetAsync(string resource, CancellationToken cancellationToken = default);

    Task AddOrUpdateAsync(string resource, CancellationToken cancellationToken = default);
}
