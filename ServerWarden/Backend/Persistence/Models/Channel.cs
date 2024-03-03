namespace Backend.Persistence.Models;

public class Channel {
  public int Id { get; init; }
  
  [MaxLength(100)] public string Name { get; init; } = null!;
  [MaxLength(5)] public string Type { get; init; } = null!;
  public int Position { get; init; }

  public List<RoleOverwrite> RoleOverwrites { get; } = new();
  public List<Message> Messages { get; } = new();
    
  public int CategoryId { get; init; }
  public Category Category { get; init; } = null!;
}

public enum ChannelType {
  Text,
  Voice,
  Forum,
  News,
  Stage
}