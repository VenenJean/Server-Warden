namespace Backend.Persistence.Models;

public class RoleOverwrite {
  public int Id { get; init; }
  
  [MaxLength(100)] public string RoleName { get; init; } = null!;
  public ulong AllowValue { get; init; }
  public ulong DenyValue { get; init; }

  public int CategoryId { get; init; }
  public Category Category { get; init; } = null!;

  public int? ChannelId { get; init; }
  public Channel? Channel { get; init; }
}