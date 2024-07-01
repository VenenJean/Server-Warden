using Discord;
using Discord.WebSocket;

namespace Application.Embeds.Archived;

public class StateEmbeds {
  private static readonly EmbedBuilder EmbedTemplate = new() {
    Author = new EmbedAuthorBuilder {
      IconUrl = "attachment://"
    },
    Footer = new EmbedFooterBuilder {
      Text = "«Developed by VenenJean»",
      IconUrl = "attachment://venenjean.jpg"
    }
  };
  private static readonly FileAttachment FooterImg = new("Images/venenjean.jpg");
  public static SocketSlashCommand Command;
  
  public static async Task SendSuccessEmbedAsync(string backupCode) {
    const string fileName = "check.png";

    EmbedTemplate.Description = $"Backup stored under the **backup_code:** `{ backupCode }`.\n\n**Features**\n```js\n/load { backupCode }\n/unload { backupCode }\n/info { backupCode }\n```";
    EmbedTemplate.Color = new Color(136, 201, 65); // Green
    EmbedTemplate.Author.Name = "Backup stored";
    EmbedTemplate.Author.IconUrl += fileName;
    
    var checkImg = new FileAttachment($"Images/Icons/{fileName}");
    await Command.RespondWithFilesAsync(new[] { checkImg, FooterImg }, embed: EmbedTemplate.Build());
  }

  public static async Task SendFailEmbedAsync(Exception ex) {
    const string fileName = "cross.png";

    EmbedTemplate.Title = ex.GetType().ToString();
    EmbedTemplate.Description = $"### Message\n```js\n{ ex.Message }\n```\n### Stack Trace\n```js\n{ ex.StackTrace ?? "No stack trace." }\n```\n**If you're clueless about the error, contact the developer.**";
    EmbedTemplate.Color = new Color(244, 67, 54); // Red
    EmbedTemplate.Author.Name = "Backup failed";
    EmbedTemplate.Author.IconUrl += fileName;
    
    var crossImg = new FileAttachment($"Images/Icons/{fileName}");
    await Command.RespondWithFilesAsync(new[] { crossImg, FooterImg }, embed: EmbedTemplate.Build());
  }

  public static async Task SendProcessEmbedAsync(string processMsg) {
    const string fileName = "question.png";

    EmbedTemplate.Color = new Color(255, 193, 7); // Yellow
    EmbedTemplate.Author.Name = processMsg;
    EmbedTemplate.Author.IconUrl += fileName;

    var questionImg = new FileAttachment($"Images/Icons/{fileName}");
    await Command.RespondWithFilesAsync(new[] { questionImg, FooterImg }, embed: EmbedTemplate.Build());
  }
}