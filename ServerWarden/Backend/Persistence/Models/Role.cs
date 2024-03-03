namespace Backend.Persistence.Models;

public class Role {
  public int Id { get; init; }
  
  [MaxLength(100)] public string Name { get; init; } = null!;
  public uint Color { get; init; }
  public int Position { get; init; }
  public bool IsHoisted { get; init; }
  public bool IsMentionable { get; init; }
  public ulong BitwisePermissionFlag { get; init; }
    
  public int GuildId { get; init; }
  public Guild Guild { get; init; } = null!;
}