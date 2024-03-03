using System.Diagnostics;
using System.Text.RegularExpressions;
using Backend.Commands.Interfaces;

namespace Backend.Commands;

public class ExecuteInsitux : IMessageCommand {
  public MessageCommandBuilder Command => new MessageCommandBuilder()
    .WithName("Execute Insitux");

  public async Task ExecuteAsync(SocketMessageCommand command) {
    await command.DeferAsync();

    var content = command.Data.Message.Content;

    var matchGroups = Regex.Match(content, @"^```clj\n([\s\S]+)\n```$").Groups;

    var code = matchGroups.Count < 2 ? content : matchGroups[1].Value;
    var fileName = $"{command.Data.Message.Id}.ix";

    await File.WriteAllTextAsync(fileName, code ?? ":nothing");

    try {
      var process = new Process {
        StartInfo = new ProcessStartInfo("bunx") {
          Arguments = $"insitux@latest -s -nc {fileName}",
          RedirectStandardOutput = true,
          UseShellExecute = false,
        }
      };

      process.Start();

      string? stdout = null;
      var thread = new Thread(() => { stdout = process.StandardOutput.ReadToEnd(); });
      thread.Start();

      await process.WaitForExitAsync();
      thread.Join();

      var sanitisedResult = stdout == ""
        ? "There was no output but I heard you."
        : $"```\n{ stdout?.Replace('`', '\'') ?? ":nothing" }\n```";

      await command.FollowupAsync(sanitisedResult);
    } finally {
      File.Delete(fileName);
    }
  }
}