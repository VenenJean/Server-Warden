using Discord.WebSocket;
using Domain.Models;

namespace Application.BotCommands.Helpers.Backup;

public class RoleLoader(SocketGuild discordGuild, Guild guildModel)
{
    private SocketGuild DiscordGuild { get; } = discordGuild;
    private Guild GuildModel { get; } = guildModel;

    public void LoadRoles()
    {
        foreach (var role in DiscordGuild.Roles)
        {
            if (role.IsManaged) continue;

            Role roleObj;
            if (!role.IsEveryone)
            {
                roleObj = new Role
                {
                    Name = role.Name,
                    Color = role.Color.RawValue,
                    Position = role.Position,
                    IsHoisted = role.IsHoisted,
                    IsMentionable = role.IsMentionable,
                    BitwisePermissionFlag = role.Permissions.RawValue,
                    Guild = GuildModel
                };
            }
            else
            {
                roleObj = new Role
                {
                    Name = "@everyone",
                    BitwisePermissionFlag = role.Permissions.RawValue
                };
            }

            GuildModel.Roles.Add(roleObj);
        }
    }
}