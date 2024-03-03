using Backend.Commands.Interfaces;
using Backend.Commands.Utility.Restore;
using Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Commands;

public class Restore : ISlashCommand {
  public SlashCommandBuilder Command { get; } = new SlashCommandBuilder()
    .WithName("restore")
    .WithDescription("Restores server from backup by backup_code.")
    .AddOption(
      "backup_code",
      ApplicationCommandOptionType.String,
      "Backup code to restore your server from.");
  
  public async Task ExecuteAsync(SocketSlashCommand command) {
    if (!command.GuildId.HasValue) {
      await command.RespondAsync("Command is only allowed to be called from a guild.");
      return;
    }
    
    var backupCode = command.Data.Options
      .FirstOrDefault(opt => opt.Name == "backup_code");

    if (backupCode == null) {
      await command.RespondAsync("No backup code provided.");
      return;
    }

    await using var context = new BackupContext();
    var modelGuild = await context.Guilds
      .Include(server => server.Roles)
      .Include(server => server.Categories)
      .ThenInclude(category => category.RoleOverwrites)
      .Include(server => server.Categories)
      .ThenInclude(category => category.Channels)
      .ThenInclude(channel => channel.RoleOverwrites)
      .Include(server => server.Categories)
      .ThenInclude(category => category.Channels)
      .ThenInclude(channel => channel.Messages)
      .SingleOrDefaultAsync(guild => guild.Id.ToString() == backupCode.Value.ToString());

    await command.RespondAsync("Backup restoration process started...");
    
    if (modelGuild != null) {
      var discordGuild = DiscordFactory.GetGuild(command.GuildId.Value);
      var discordWriter = new DiscordWriter(modelGuild, discordGuild);
      await discordWriter.RestoreGuildAsync();
    } else {
      await command.FollowupAsync($"Backup with backup code **{backupCode.Value}** not found.");
    }
  }
}