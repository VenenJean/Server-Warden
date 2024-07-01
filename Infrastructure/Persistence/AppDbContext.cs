using Domain.Models;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleOverwrite> RoleOverwrites { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<MemberRole> MemberRoles { get; set; }

    // TODO: Implement ConfigManager
    // private readonly string _connectionString =
    //   $"server={ConfigManager.ReadSetting("DbHost")};" +
    //   $"port={ulong.Parse(ConfigManager.ReadSetting("DbPort"))};" +
    //   $"user={ConfigManager.ReadSetting("DbUser")};" +
    //   $"password={ConfigManager.ReadSetting("DbUserPass")};" +
    //   $"database={ConfigManager.ReadSetting("DbName")};";

    private readonly string _connectionString =
        $"server=localhost;" +
        $"port=3306;" +
        $"user=root;" +
        $"password=Antidote2580134679#;" +
        $"database=DiscordBackups;";

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(_connectionString, new MariaDbServerVersion(new Version(11, 2, 2)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GuildConfiguration());
        modelBuilder.ApplyConfiguration(new EntityConfiguration<Role>());
        modelBuilder.ApplyConfiguration(new EntityConfiguration<Category>());
        modelBuilder.ApplyConfiguration(new EntityConfiguration<Channel>());
        modelBuilder.ApplyConfiguration(new EntityConfiguration<Message>());
        modelBuilder.ApplyConfiguration(new EntityConfiguration<RoleOverwrite>());
        modelBuilder.ApplyConfiguration(new EntityConfiguration<MemberRole>());
        modelBuilder.ApplyConfiguration(new EntityConfiguration<Member>());
    }
}