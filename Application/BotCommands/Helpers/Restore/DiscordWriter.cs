using Discord;
using Discord.WebSocket;
using Domain.Models;

namespace Application.BotCommands.Helpers.Restore;

public class DiscordWriter(Guild dbGuild, SocketGuild discordGuild)
{
    public async Task RestoreGuildAsync()
    {
        var serverCleaner = new ServerCleaner(discordGuild);
        await serverCleaner.CleanServer();
        await discordGuild.ModifyAsync(props => { props.Name = dbGuild.Name; });
        await RestoreRoles();
        await RestoreCategories();
    }

    private async Task RestoreRoles()
    {
        var orderedRoles = dbGuild.Roles.OrderByDescending(role => role.Position);
        foreach (var role in orderedRoles)
        {
            if (role.Name == "@everyone")
            {
                var everyoneRole = discordGuild.Roles.First(socketRole => socketRole.IsEveryone);
                await everyoneRole.ModifyAsync(props =>
                {
                    props.Permissions = new GuildPermissions(role.BitwisePermissionFlag);
                });
                continue;
            }

            await discordGuild.CreateRoleAsync(
                role.Name,
                new GuildPermissions(role.BitwisePermissionFlag),
                new Color(role.Color),
                role.IsHoisted,
                role.IsMentionable
            );
        }
    }

    private async Task RestoreCategories()
    {
        foreach (var category in dbGuild.Categories)
        {
            var categoryChannel = await discordGuild.CreateCategoryChannelAsync(
                category.Name,
                props => { props.Position = category.Position; });

            await categoryChannel.ModifyAsync(props =>
            {
                props.PermissionOverwrites = CreateOverwriteObj(category.RoleOverwrites);
            });
            await RestoreChannels(category.Channels, categoryChannel.Id);
        }
    }

    private async Task RestoreChannels(List<Channel> channels, ulong categoryId)
    {
        foreach (var channel in channels)
        {
            var channelPermissionOverwrites = CreateOverwriteObj(channel.RoleOverwrites);

            switch (channel.Type)
            {
                case "Text":
                {
                    await CreateTextChannel(channel.Name, channel.Position, channelPermissionOverwrites, categoryId);
                    break;
                }
                case "Voice":
                {
                    await discordGuild.CreateVoiceChannelAsync(
                        channel.Name,
                        props =>
                        {
                            props.Position = channel.Position;
                            props.PermissionOverwrites = channelPermissionOverwrites;
                            props.CategoryId = categoryId;
                        });
                    break;
                }
                case "Forum":
                {
                    try
                    {
                        await discordGuild.CreateForumChannelAsync(
                            channel.Name,
                            props =>
                            {
                                props.Position = channel.Position;
                                props.PermissionOverwrites = channelPermissionOverwrites;
                                props.CategoryId = categoryId;
                            });
                    }
                    catch (Discord.Net.HttpException)
                    {
                        Console.WriteLine($"{channel.Type} not supported on {discordGuild.Name}");
                        await CreateTextChannel(channel.Name, channel.Position, channelPermissionOverwrites,
                            categoryId);
                    }

                    break;
                }
                case "News":
                {
                    await CreateTextChannel(channel.Name, channel.Position, channelPermissionOverwrites, categoryId);
                    break;
                }
                case "Stage":
                {
                    try
                    {
                        await discordGuild.CreateStageChannelAsync(
                            channel.Name,
                            props =>
                            {
                                props.Position = channel.Position;
                                props.PermissionOverwrites = channelPermissionOverwrites;
                                props.CategoryId = categoryId;
                            });
                    }
                    catch (Discord.Net.HttpException)
                    {
                        Console.WriteLine($"{channel.Type} not supported on {discordGuild.Name}");
                        await CreateTextChannel(channel.Name, channel.Position, channelPermissionOverwrites,
                            categoryId);
                    }

                    break;
                }
            }

            if (channel.Messages.Any())
            {
                await RestoreMessages(channel);
            }
        }
    }

    private async Task RestoreMessages(Channel channel)
    {
        foreach (var message in channel.Messages.OrderBy(date => date.SentTime))
        {
            try
            {
                var chan = (IMessageChannel)discordGuild.Channels.First(channel1 =>
                    channel1.Name == message.Channel.Name);
                await chan.SendMessageAsync(message.Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    private async Task CreateTextChannel(string name, int position,
        Optional<IEnumerable<Overwrite>> channelPermissionOverwrites, ulong categoryId)
    {
        await discordGuild.CreateTextChannelAsync(
            name,
            props =>
            {
                props.Position = position;
                props.PermissionOverwrites = channelPermissionOverwrites;
                props.CategoryId = categoryId;
            });
    }

    private Optional<IEnumerable<Overwrite>> CreateOverwriteObj(List<RoleOverwrite> channelOverwrites)
    {
        List<Overwrite> overwrites = [];

        foreach (var roleOverwrite in channelOverwrites)
        {
            var targetRole = discordGuild.Roles.FirstOrDefault(role => role.Name == roleOverwrite.RoleName);
            if (targetRole is null) continue;

            var overwritePermissions = new OverwritePermissions(roleOverwrite.AllowValue, roleOverwrite.DenyValue);
            var overwrite = new Overwrite(targetRole.Id, PermissionTarget.Role, overwritePermissions);
            overwrites.Add(overwrite);
        }

        var optional = new Optional<IEnumerable<Overwrite>>(overwrites);
        return optional;
    }
}