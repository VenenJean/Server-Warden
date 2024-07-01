using Discord;
using Discord.WebSocket;

namespace Application.Abstractions;

public interface ISlashCommand : ICommand<SocketSlashCommand, SlashCommandBuilder>;