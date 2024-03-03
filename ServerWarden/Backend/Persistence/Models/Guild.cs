namespace Backend.Persistence.Models;

public class Guild {
  public int Id { get; init; }
  // [MaxLength(36)] public string BackupCode { get; } = new Guid().ToString();
  [MaxLength(100)] public string Name { get; init; } = null!;
  [MaxLength(120)] public string Description { get; init; } = null!;

  public List<Category> Categories { get; } = new();
  public List<Role> Roles { get; } = new();
}