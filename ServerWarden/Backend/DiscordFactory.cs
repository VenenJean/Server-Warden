namespace Backend;

public class DiscordFactory {
  private static readonly Lazy<DiscordSocketClient> ClientInstance = new(() => {
    try {
      var client = new DiscordSocketClient(new DiscordSocketConfig {
        GatewayIntents = GatewayIntents.All & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites & ~GatewayIntents.GuildPresences
      });
        
      return client;
    } catch (Exception ex) {
      Console.WriteLine($"Error initializing DiscordSocketClient: { ex.Message }");
      throw;
    }
  });

  public static DiscordSocketClient GetClient() => ClientInstance.Value;
  public static SocketGuild GetGuild(ulong guildId) {
    try {
      return ClientInstance.Value.GetGuild(guildId);
    } catch (Exception ex) {
      Console.WriteLine($"Error getting SocketGuild: { ex.Message }");
      throw;
    }
  }
}