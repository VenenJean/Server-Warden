using Backend.Commands.Embeds;
using Backend.Commands.Interfaces;
using Backend.Commands.Utility.Backup;

namespace Backend.Commands;

public class Backup : ISlashCommand {
  public SlashCommandBuilder Command { get; } = new SlashCommandBuilder()
    .WithName("backup")
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

    var backupEmbed = new BackupEmbed(command);
    await backupEmbed.SendBackupEmbedAsync();
  }

  public static async Task StartBackupProcess(SocketMessageComponent component) {
    await component.ModifyOriginalResponseAsync(props => {
      props.Content = "Processing backup data...";
    });

    var modelLoader = new ModelLoader(DiscordFactory.GetGuild(component.GuildId.Value));
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