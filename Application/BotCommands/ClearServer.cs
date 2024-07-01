using Application.Abstractions;
using Application.BotCommands.Helpers.Restore;
using Discord;
using Discord.WebSocket;

namespace Application.BotCommands;

public class ClearServer : ISlashCommand
{
    public SlashCommandBuilder Command { get; } = new SlashCommandBuilder()
        .WithName("clear-server")
        .WithDescription("Deletes all channels & roles from the server.")
        .WithDefaultMemberPermissions(GuildPermission.Administrator);
    
    public async Task ExecuteAsync(SocketSlashCommand command)
    {
        if (command.GuildId == null)
        {
            await command.RespondAsync("A problem occured!.");
            return;
        }

        await command.RespondAsync("Cleaning started...");
        await new ServerCleaner(DiscordFactory.GetGuild(command.GuildId.Value)).CleanServer();
    }
}