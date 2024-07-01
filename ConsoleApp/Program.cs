using Application;
using Application.Handlers;
using Application.Listeners;
using ConsoleApp.Logging;
using Discord;
using Discord.WebSocket;
using Infrastructure;

namespace ConsoleApp;

internal class Program
{
    private readonly DiscordSocketClient _client = DiscordFactory.GetClient();
    private static readonly Logger Logger = new();
    private readonly CommandHandler _commandHandler = new(new GuildRepository(), Logger);
    private readonly SelectMenuHandler _selectMenuHandler = new(Logger);
    private readonly ButtonHandler _buttonHandler = new(Logger);

    public static async Task Main(string[] args)
    {
        await new Program().MainAsync(args);
    }

    private async Task MainAsync(string[] args)
    {
        _commandHandler.StoreCommandsLocally();

        _client.Log += Logger.LogClientMessage;
        _client.SlashCommandExecuted += _commandHandler.HandleCommandAsync;
        _client.MessageCommandExecuted += _commandHandler.HandleCommandAsync;
        _client.SelectMenuExecuted += _selectMenuHandler.HandleSelectMenuAsync;
        _client.ButtonExecuted += _buttonHandler.HandleButtonAsync;
        _client.JoinedGuild += JoinedGuild;
        _client.MessageDeleted += new MessageListener().HandleMessageDeleted;

        await _client.LoginAsync(TokenType.Bot, new ConfigManager().ReadSetting("BotToken"));
        await _client.StartAsync();

        _client.Ready += Ready;

        await Task.Delay(-1);
    }

    private async Task JoinedGuild(SocketGuild guild)
    {
        await LoadCommandsToGuildsAsync();
    }

    private async Task LoadCommandsToGuildsAsync()
    {
        foreach (var guild in _client.Guilds)
        {
            await guild.DeleteApplicationCommandsAsync();
            await _commandHandler.LoadCommandsAsync(_client, guild);
        }
    }

    private Task Ready()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                _client.Ready -= Ready;
                await LoadCommandsToGuildsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogException("Program.cs", "Read()", ex);
            }
        });

        return Task.CompletedTask;
    }
}