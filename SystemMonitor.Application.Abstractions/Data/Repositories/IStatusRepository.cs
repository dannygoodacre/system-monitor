using SystemMonitor.Domain.Models;

namespace SystemMonitor.Application.Abstractions.Data.Repositories;

public interface IStatusRepository
{
    void Add(Status status);

    Task<Status> GetPreviousAsync(string condition, CancellationToken cancellationToken = default);
}
