using Application.Abstractions;
using Domain.Abstractions;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class GuildRepository : IGuildRepository
{
    private AppDbContext DbContext { get; } = new();

    public GuildRepository()
    {
        DbContext.Database.EnsureCreatedAsync();
    }

    public async Task<Guild?> GetGuildById(string backupCode)
    {
        var dbGuild = await new AppDbContext().Guilds
            .Include(server => server.Roles)
            .Include(server => server.Categories)
            .ThenInclude(category => category.RoleOverwrites)
            .Include(server => server.Categories)
            .ThenInclude(category => category.Channels)
            .ThenInclude(channel => channel.RoleOverwrites)
            .Include(server => server.Categories)
            .ThenInclude(category => category.Channels)
            .ThenInclude(channel => channel.Messages)
            .SingleOrDefaultAsync(guild => guild.Id.ToString() == backupCode);

        return dbGuild;
    }

    public async Task AddAsync(Entity entity)
    {
        await DbContext.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<Entity> entities)
    {
        await DbContext.AddRangeAsync(entities);
    }

    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}