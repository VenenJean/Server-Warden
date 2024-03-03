using Backend.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Persistence;

public class BackupContext : DbContext {
  public DbSet<Guild> Guilds { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<Channel> Channels { get; set; }
  public DbSet<Message> Messages { get; set; }
  public DbSet<Role> Roles { get; set; }
  public DbSet<RoleOverwrite> RoleOverwrites { get; set; }

  private readonly string _connectionString =
    $"server={ConfigManager.ReadSetting("DbHost")};" +
    $"port={ulong.Parse(ConfigManager.ReadSetting("DbPort"))};" +
    $"user={ConfigManager.ReadSetting("DbUser")};" +
    $"password={ConfigManager.ReadSetting("DbUserPass")};" +
    $"database={ConfigManager.ReadSetting("DbName")};";
  
  public BackupContext() { }
  public BackupContext(DbContextOptions<BackupContext> options) : base(options) { }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    optionsBuilder.UseMySql(_connectionString, new MariaDbServerVersion(new Version(11, 2, 2)));
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Guild>()
      .HasIndex(e => e.Name)
      .IsUnique();

    modelBuilder.Entity<Message>()
      .HasIndex(e => e.Content)
      .IsUnique();
  }
}