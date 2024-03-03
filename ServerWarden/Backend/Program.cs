using Backend.Commands.Handlers;
using static Backend.ConfigManager;

namespace Backend;

internal class Program {
  private readonly DiscordSocketClient _client = DiscordFactory.GetClient();
  private readonly Logger _logger = new();
  private readonly CommandHandler _commandHandler = new();
  private readonly SelectMenuHandler _selectMenuHandler = new();
  private readonly ButtonHandler _buttonHandler = new();

  public static async Task Main(string[] args) {
    await new Program().MainAsync(args);
  }

  private async Task MainAsync(string[] args) {
    _commandHandler.StoreCommandsLocally();

    _client.Log += _logger.LogClientMessage;
    _client.SlashCommandExecuted += _commandHandler.HandleCommandAsync;
    _client.MessageCommandExecuted += _commandHandler.HandleCommandAsync;
    _client.SelectMenuExecuted += _selectMenuHandler.HandleSelectMenuAsync;
    _client.ButtonExecuted += _buttonHandler.HandleButtonAsync;

    await _client.LoginAsync(TokenType.Bot, ReadSetting("BotToken"));
    await _client.StartAsync();

    _client.Ready += Ready;

    await Task.Delay(-1);
  }

  private Task Ready() {
    _ = Task.Run(async () => {
      try {
        _client.Ready -= Ready;

        foreach (var guild in _client.Guilds) {
          await _commandHandler.LoadCommandsAsync(_client, guild);
        }
      } catch (Exception ex) {
        Logger.LogException("Program.cs", "Read()", ex);
      }
    });

    return Task.CompletedTask;
  }
}