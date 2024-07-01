using Application.Abstractions;
using Discord.WebSocket;
using Domain.Models;

namespace Application.BotCommands.Helpers.Backup;

public class ModelLoader
{
    /* States
     * filled => ok
     * both / one is new() => throw error
     * null => throw error
     */
    public static List<string>? ChannelsForMessages { get; set; } = [];
    public static List<string>? RolesForUsers { get; set; } = [];

    private readonly SocketGuild _discordGuild;
    public readonly Guild GuildModel;
    private IGuildRepository GuildRepository { get; }

    public ModelLoader(IGuildRepository guildRepository, SocketGuild discordGuild)
    {
        _discordGuild = discordGuild;
        GuildRepository = guildRepository;
        GuildModel = new Guild
        {
            Name = _discordGuild.Name,
            Description = _discordGuild.Description ?? string.Empty,
            GuildId = discordGuild.Id.ToString()
        };
    }

    public async Task<bool> LoadDataAsync()
    {
        new RoleLoader(_discordGuild, GuildModel).LoadRoles();
        await new CategoryLoader(_discordGuild, GuildModel, ChannelsForMessages, RolesForUsers).LoadCategoriesAsync();

        if (ChannelsForMessages == null || RolesForUsers == null) return false;
        if (ChannelsForMessages.Count == 0 || RolesForUsers.Count == 0) return false;

        await GuildRepository.AddAsync(GuildModel);
        await GuildRepository.AddRangeAsync(GuildModel.Roles);
        await GuildRepository.AddRangeAsync(GuildModel.Categories);

        foreach (var category in GuildModel.Categories)
        {
            await GuildRepository.AddRangeAsync(category.Channels);

            foreach (var channel in category.Channels)
            {
                await GuildRepository.AddRangeAsync(channel.Messages);
                await GuildRepository.AddRangeAsync(channel.RoleOverwrites);
            }

            await GuildRepository.AddRangeAsync(category.RoleOverwrites);
        }

        await GuildRepository.SaveChangesAsync();
        return true;
    }
}