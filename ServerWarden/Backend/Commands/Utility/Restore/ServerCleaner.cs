namespace Backend.Commands.Utility.Restore;

public class ServerCleaner {
  private SocketGuild _guild = null!;

  public async Task CleanServer(SocketGuild guild) {
    _guild = guild;
    
    await DeleteRoles();
    await DeleteChannels();
  }

  private async Task DeleteRoles() {
    var rolesToDelete = _guild.Roles.Where(role => !role.IsEveryone && !role.IsManaged);
    foreach (var role in rolesToDelete) {
      await role.DeleteAsync();
    }
  }

  private async Task DeleteChannels() {
    foreach (var channel in _guild.Channels) {
      try {
        await channel.DeleteAsync();
      } catch (Discord.Net.HttpException ex) {
        Console.WriteLine($"{channel.Name} : {ex.Message}");
      }
    }
  }
}