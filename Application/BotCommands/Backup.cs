using Application.Abstractions;
using Application.BotCommands.Helpers.Backup;
using Application.Embeds;
using Discord;
using Discord.WebSocket;

namespace Application.BotCommands;

public class Backup : ISlashCommand {
  private static IGuildRepository GuildRepository { get; set; }
  public Backup(IGuildRepository guildRepository) {
    GuildRepository = guildRepository;
  }
  
  public SlashCommandBuilder Command { get; } = new SlashCommandBuilder()
    .WithName("backup-server")
    .WithDescription("Creates a profile backup of current server.")
    .AddOption(
      "category",
      ApplicationCommandOptionType.Channel,
      "The category you want to backup channel messages from");
  
  public async Task ExecuteAsync(SocketSlashCommand command) {
    if (!command.GuildId.HasValue) {
      await command.RespondAsync("Command is only allowed to be called from a guild.");
      return;
    }

    var backupEmbed = new BackupEmbed(command, command.GuildId.Value);
    await backupEmbed.SendBackupEmbedAsync();
  }

  public static async Task StartBackupProcess(SocketMessageComponent component) {
    await component.ModifyOriginalResponseAsync(props => {
      props.Content = "Processing backup data...";
    });

    var modelLoader = new ModelLoader(GuildRepository, DiscordFactory.GetGuild(component.GuildId.Value));
    var backupSuccess = false;

    await Task.Run(async () => {
      backupSuccess = await modelLoader.LoadDataAsync();
    });

    if (backupSuccess) {
      await component.ModifyOriginalResponseAsync(props => {
        props.Content = $"Backup stored.\n```cs\n/restore {modelLoader.GuildModel.Id}\n```";
      });
      return;
    }

    await component.ModifyOriginalResponseAsync(props => {
        props.Content = "Backup failed.";
      }
    );
  }
}