namespace Application.Abstractions;

public interface ICommand<in TCommand, out TCommandBuilder> {
  TCommandBuilder Command { get; }

  Task ExecuteAsync(TCommand command);
}