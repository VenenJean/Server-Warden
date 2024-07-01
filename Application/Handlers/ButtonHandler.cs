using Application.Abstractions;
using Application.BotCommands;
using Discord.WebSocket;

namespace Application.Handlers;

public class ButtonHandler(ILogger logger) {
  public Task HandleButtonAsync(SocketMessageComponent component) {
    _ = Task.Run(async () => {
      try {
        await component.Message.DeleteAsync();
        await component.RespondAsync("Handling interaction...", ephemeral: true);

        var buttonId = component.Data.CustomId;

        switch (buttonId) {
          case "backupButton":
            await Backup.StartBackupProcess(component);

            break;
          default:
            var defaultMsg = $"ButtonHandler: '{buttonId}' interaction isn't handled.";
            if (component.HasResponded) {
              await component.ModifyOriginalResponseAsync(props => { props.Content = defaultMsg; });
            }
            else {
              await component.RespondAsync(defaultMsg);
            }

            break;
        }
      } catch (Exception ex) {
        logger.LogException("ButtonHandler.cs", "HandleButtonAsync()", ex);
      }
    });

    return Task.CompletedTask;
  }
}