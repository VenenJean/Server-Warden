using Application.Abstractions;
using Discord;
using Discord.WebSocket;

namespace Application.BotCommands;

public class Tester : ISlashCommand
{
    public SlashCommandBuilder Command { get; } = new SlashCommandBuilder()
        .WithName("testserver")
        .WithDescription("Creates a profile backup of current server.")
        .AddOption(
            "category",
            ApplicationCommandOptionType.Channel,
            "The category you want to backup channel messages from");

    public async Task ExecuteAsync(SocketSlashCommand command)
    {
        await command.RespondAsync("hey there");
    }
}