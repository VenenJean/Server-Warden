namespace Backend.Persistence.Models;

public class Category {
  public int Id { get; init; }
  [MaxLength(100)] public string Name { get; init; } = null!;
  public int Position { get; init; }

  public List<RoleOverwrite> RoleOverwrites { get; } = new();
  public List<Channel> Channels { get; } = new();
    
  public int GuildId { get; init; }
  public Guild Guild { get; init; } = null!;
}