namespace Backend.Commands.Interfaces;

public interface ICommand<in TCommand, out TCommandBuilder> {
  TCommandBuilder Command { get; }

  Task ExecuteAsync(TCommand command);
}

public interface ISlashCommand : ICommand<SocketSlashCommand, SlashCommandBuilder> { }

public interface IMessageCommand : ICommand<SocketMessageCommand, MessageCommandBuilder> { }