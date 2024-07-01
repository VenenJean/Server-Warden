using Discord.WebSocket;
using Domain.Models;

namespace Application.BotCommands.Helpers.Backup;

public class CategoryLoader(SocketGuild discordGuild, Guild guildModel, List<string>? ChannelsForMessages, List<string>? RolesForUsers)
{
    private SocketGuild DiscordGuild { get; } = discordGuild;
    private Guild GuildModel { get; } = guildModel;

    public async Task LoadCategoriesAsync()
    {
        foreach (var category in DiscordGuild.CategoryChannels)
        {
            var categoryObj = new Category
            {
                Name = category.Name,
                Position = category.Position,
                Guild = GuildModel
            };

            new OverwritesLoader(DiscordGuild).LoadRoleOverwrites(categoryObj, category.PermissionOverwrites);
            await new ChannelLoader(ChannelsForMessages, RolesForUsers, DiscordGuild).LoadChannelsAsync(categoryObj, category);

            GuildModel.Categories.Add(categoryObj);
        }
    }
}