using Domain.Abstractions;
using Domain.Models;

namespace Application.Abstractions;

public interface IGuildRepository
{
    Task<Guild?> GetGuildById(string backupCode);

    Task AddAsync(Entity entity);
    Task AddRangeAsync(IEnumerable<Entity> entities);
    Task SaveChangesAsync();
}