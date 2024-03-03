namespace Backend.Persistence.Models;

public class Message {
  public int Id { get; init; }
  
  [MaxLength(2000)] public string Content { get; init; } = null!;
    
  public int ChannelId { get; init; }
  public DateTimeOffset SentTime { get; init; }
  public Channel Channel { get; init; } = null!;
}