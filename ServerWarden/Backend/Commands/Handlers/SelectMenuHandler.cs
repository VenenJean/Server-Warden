using Backend.Commands.Utility.Backup;

namespace Backend.Commands.Handlers;

public class SelectMenuHandler {
  public Task HandleSelectMenuAsync(SocketMessageComponent arg) {
    _ = Task.Run(async () => {
      try {
        switch (arg.Data.CustomId) {
          case "channelSelect": {
            if (!await CheckValidAsync()) {
              ModelLoader.ChannelsForMessages = null;
              break;
            }

            ModelLoader.ChannelsForMessages = arg.Data.Values.ToList();
            break;
          }
          case "roleSelect": {
            if (!await CheckValidAsync()) {
              ModelLoader.RolesForUsers = null;
              break;
            }

            ModelLoader.RolesForUsers = arg.Data.Values.ToList();
            break;
          }
        }

        return;

        async Task<bool> CheckValidAsync() {
          var data = arg.Data.Values;

          if (data.Contains("defaultRoleOption") && data.Count > 1) {
            await arg.RespondAsync("You cannot select defaultOption + another option.", ephemeral: true);
            return false;
          }

          await arg.RespondAsync("Selection was saved!",
            ephemeral: true);
          return true;
        }
      } catch (Exception ex) {
        Logger.LogException("SelectMenuHandler.cs", "HandleSelectMenu", ex);
      }
    });

    return Task.CompletedTask;
  }
}