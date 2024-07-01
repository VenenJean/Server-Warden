using Application.Abstractions;
using Application.BotCommands.Helpers.Restore;
using Discord;
using Discord.WebSocket;

namespace Application.BotCommands;

public class Restore(IGuildRepository guildRepo) : ISlashCommand {
  public SlashCommandBuilder Command { get; } = new SlashCommandBuilder()
    .WithName("restore-server")
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

    if (backupCode?.Value == null) {
      await command.RespondAsync("No backup code provided.");
      return;
    }
    
    var dbGuild = await guildRepo.GetGuildById(backupCode.Value.ToString());

    await command.RespondAsync("Backup restoration process started...");
    
    if (dbGuild != null) {
      var discordGuild = DiscordFactory.GetGuild(command.GuildId.Value);
      var discordWriter = new DiscordWriter(dbGuild, discordGuild);
      await discordWriter.RestoreGuildAsync();
    } else {
      await command.FollowupAsync($"Backup with backup code **{backupCode.Value}** not found.");
    }
  }
}