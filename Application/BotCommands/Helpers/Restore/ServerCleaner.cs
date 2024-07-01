using Discord.WebSocket;

namespace Application.BotCommands.Helpers.Restore;

public class ServerCleaner(SocketGuild guild) {
  public async Task CleanServer() {
    await DeleteRoles();
    await DeleteChannels();
  }

  private async Task DeleteRoles() {
    var rolesToDelete = guild.Roles.Where(role => !role.IsEveryone && !role.IsManaged);
    foreach (var role in rolesToDelete) {
      await role.DeleteAsync();
    }
  }

  private async Task DeleteChannels() {
    foreach (var channel in guild.Channels) {
      try {
        await channel.DeleteAsync();
      } catch (Discord.Net.HttpException ex) {
        Console.WriteLine($"{channel.Name} : {ex.Message}");
      }
    }
  }
}