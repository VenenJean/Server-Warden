using Discord;
using Discord.WebSocket;

namespace Application.Embeds;

public class BackupEmbed {
  private ComponentBuilder _builder = new();
  private readonly SocketSlashCommand _command;
  private readonly ulong _guildId;

  public BackupEmbed(SocketSlashCommand command, ulong guildId) {
    _command = command;
    _guildId = guildId;
  }
  
  public async Task SendBackupEmbedAsync() {
    var backupCategoryOpt = _command.Data.Options
      .FirstOrDefault(opt => opt.Name == "category");
    
    if (backupCategoryOpt != null) {
      if (backupCategoryOpt.Value.GetType() != typeof(SocketCategoryChannel)) {
        await _command.RespondAsync("Provide a category channel.");
        return;
      }

      var category = (SocketCategoryChannel)backupCategoryOpt.Value;
      if (category.Channels.Count > 0) {
        AddChannels(category);
      }
    }
    
    if (DiscordFactory.GetGuild(_guildId).Roles.Count > 0) {
      AddRoles();
    }

    _builder.WithButton(
      new ButtonBuilder()
      .WithCustomId("backupButton")
      .WithLabel("Backup now!")
      .WithStyle(ButtonStyle.Success)
    );
    
    var guildName = DiscordFactory.GetGuild(_guildId).Name;
    await _command.RespondAsync(
      $"## Backup from { guildName } on { DateTime.Now }!\nOnly the first 25 channels / roles are shown.\nIf the desired channel / role isn't present, move it up in the hierarchy.\n",
      components: _builder.Build());
    _builder = new ComponentBuilder();
  }
  
  private void AddChannels(SocketCategoryChannel category) {
    var orderedChannels = category.Channels.OrderBy(channel => channel.Position);
    var channels = orderedChannels
      .Select(channel => new SelectMenuOptionBuilder()
        .WithLabel(channel.Name)
        .WithValue(channel.Name))
      .ToList();

    var defaultChannelOption = new SelectMenuOptionBuilder()
      .WithLabel("Nothing to backup...")
      .WithValue("defaultChannelOption");
      
    var maxValues = AdjustCollectionTo25Items(channels, defaultChannelOption);
      
    var channelSelectMenu = new SelectMenuBuilder()
      .WithCustomId("channelSelect")
      .WithPlaceholder("Select channels to backup messages from...")
      .WithMinValues(1)
      .WithMaxValues(maxValues)
      .WithDefaultValues()
      .WithOptions(channels);

    _builder.WithSelectMenu(channelSelectMenu);
  }

  private void AddRoles() {
    var orderedRoles = DiscordFactory.GetGuild(_guildId).Roles.OrderByDescending(role => role.Position);
    var roles = orderedRoles
      .Where(role => role.IsManaged == false)
      .Select(role => new SelectMenuOptionBuilder()
        .WithLabel(role.Name)
        .WithValue(role.Id.ToString()))
      .ToList();
      
    var defaultRoleOption = new SelectMenuOptionBuilder()
      .WithLabel("Nothing to backup...")
      .WithValue("defaultRoleOption");
      
    var maxValues = AdjustCollectionTo25Items(roles, defaultRoleOption);
      
    var roleSelectMenu = new SelectMenuBuilder()
      .WithCustomId("roleSelect")
      .WithPlaceholder("Select important user-role relationships to backup...")
      .WithMinValues(1)
      .WithMaxValues(maxValues)
      .WithOptions(roles);

    _builder.WithSelectMenu(roleSelectMenu);
  }


  private int AdjustCollectionTo25Items<T>(IList<T> collection, T defaultOption) {
    collection.Insert(0, defaultOption);

    if (collection.Count > 25) {
      do {
        collection.RemoveAt(collection.Count - 1);
      } while (collection.Count != 25);
    } else if (collection.Count < 4) {
      return collection.Count - 1;
    }

    return 5; // Default max. amount of selections
  }
}