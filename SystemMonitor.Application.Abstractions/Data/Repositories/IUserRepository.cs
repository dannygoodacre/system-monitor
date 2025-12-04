using SystemMonitor.Domain.Models;

namespace SystemMonitor.Application.Abstractions.Data.Repositories;

public interface IUserRepository
{
    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
}
