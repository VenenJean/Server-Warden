using Discord;
using Discord.WebSocket;
using Domain.Models;

namespace Application.BotCommands.Helpers.Backup;

public class ChannelLoader(List<string> channelsForMessages, List<string> rolesForUsers, SocketGuild discordGuild)
{
    private List<string>? ChannelsForMessages { get; set; } = channelsForMessages;
    public List<string>? RolesForUsers { get; set; } = rolesForUsers;
    private SocketGuild DiscordGuild { get; } = discordGuild;

    public async Task LoadChannelsAsync(Category dbCategory, SocketCategoryChannel discordCategory)
    {
        foreach (var channel in discordCategory.Channels)
        {
            var channelObj = new Channel
            {
                Name = channel.Name,
                Position = channel.Position,
                Type = channel.GetChannelType().Value.ToString(),
                Category = dbCategory
            };

            await LoadMessagesAsync(channelObj, channel);
            new OverwritesLoader(DiscordGuild).LoadRoleOverwrites(dbCategory, channelObj, channel.PermissionOverwrites);

            dbCategory.Channels.Add(channelObj);
        }
    }
    
    private async Task LoadMessagesAsync(Channel dbChannel, SocketGuildChannel discordChannel)
    {
        if (ChannelsForMessages != null && ChannelsForMessages.Contains(discordChannel.Name) &&
            discordChannel is ITextChannel textChannel)
        {
            var messages = await textChannel.GetMessagesAsync().FlattenAsync();

            foreach (var message in messages)
            {
                const int maxMsgLength = 2000;
                if (message.Content == string.Empty || message.Content.Length > maxMsgLength) continue;
                var messageObj = new Message
                {
                    Content = message.Content,
                    SentTime = message.Timestamp
                };
                dbChannel.Messages.Add(messageObj);
            }

            dbChannel.Messages.Reverse();
        }
    }
}