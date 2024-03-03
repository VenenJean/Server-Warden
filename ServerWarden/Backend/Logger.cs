namespace Backend;

public class Logger {
  public Task LogClientMessage(LogMessage logMessage) {
    _ = Task.Run(() => {
      if (logMessage.Exception is CommandException commandException) {
        Console.WriteLine($"[Command/{logMessage.Severity}] {commandException.Command.Aliases[0]}" +
                          $" failed to execute in {commandException.Context.Channel}.");
        Console.WriteLine($"{commandException}");
      } else {
        Console.WriteLine($"[General/{logMessage.Severity}] {logMessage}");
      }
    });

    return Task.CompletedTask;
  }

  public static void LogException(string fileName, string methodName, Exception ex) {
    var innerException = "No inner exception";
    if (ex.InnerException is not null) {
      innerException = $"{ex.InnerException.GetType()}\n{ex.InnerException.Message}\n{ex.InnerException.StackTrace}";
    }

    Console.WriteLine($"[{fileName} : {methodName}] {ex.GetType()}\n" +
                      $"{ex.Message}\n" +
                      $"{innerException}\n" +
                      $"{ex.StackTrace}");
  }
}