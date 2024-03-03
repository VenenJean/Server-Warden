using Backend.Commands.Interfaces;

namespace Backend.Commands.Handlers;

public class CommandHandler {
  private readonly IDictionary<string, ISlashCommand> _localSlashCommands = 
    new Dictionary<string, ISlashCommand>();

  private readonly IDictionary<string, IMessageCommand> _localMessageCommands =
    new Dictionary<string, IMessageCommand>();

  public void StoreCommandsLocally() {
    AddCommands(new List<ISlashCommand> {
      new Backup(),
      new Restore()
    });
    
    AddCommands(new List<IMessageCommand> {
      new ExecuteInsitux()
    });
  }

  private void AddCommands(List<ISlashCommand> commands) {
    foreach (var command in commands) {
      _localSlashCommands.Add(command.Command.Name, command);
    }
  }
  
  private void AddCommands(List<IMessageCommand> commands) {
    foreach (var command in commands) {
      _localMessageCommands.Add(command.Command.Name, command);
    }
  }

  public async Task LoadCommandsAsync(DiscordSocketClient client, SocketGuild guild) {
    foreach (var slashCommand in _localSlashCommands.Values.ToList()) {
      await guild.CreateApplicationCommandAsync(slashCommand.Command.Build());
    }

    foreach (var messageCommand in _localMessageCommands.Values.ToList()) {
      await client.Rest.CreateGuildCommand(messageCommand.Command.Build(), guild.Id);
    }
  }

  public Task HandleCommandAsync(SocketSlashCommand command) {
    _ = Task.Run(async () => {
      try {
        if (_localSlashCommands.TryGetValue(command.Data.Name, out var slashCommand)) {
          await Task.Run(async () => {
            await slashCommand.ExecuteAsync(command);
          });
        }
      } catch (Exception ex) {
        Logger.LogException("CommandHandler.cs", "HandleCommandAsync(SocketSlashCommand)", ex);
      }
    });

    return Task.CompletedTask;
  }
  
  public Task HandleCommandAsync(SocketMessageCommand command) {
    _ = Task.Run(async () => {
      try {
        if (_localMessageCommands.TryGetValue(command.Data.Name, out var messageCommand)) {
          await messageCommand.ExecuteAsync(command);
        }
      } catch (Exception ex)  {
        Logger.LogException("CommandHandler.cs", "HandleCommandAsync(SocketMessageCommand)", ex);
      }
    });

    return Task.CompletedTask;
  }
}