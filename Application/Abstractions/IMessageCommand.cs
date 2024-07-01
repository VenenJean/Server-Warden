using Discord;
using Discord.WebSocket;

namespace Application.Abstractions;

public interface IMessageCommand : ICommand<SocketMessageCommand, MessageCommandBuilder>;
