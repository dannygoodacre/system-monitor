using Microsoft.EntityFrameworkCore;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Data.Extensions;
using SystemMonitor.Domain.Models;

namespace SystemMonitor.Data.Repositories;

public class UserRepository(ApplicationContext context) : IUserRepository
{
    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        => context.Users
            .Select(x => x.ToModel())
            .ToListAsync(cancellationToken);
}
