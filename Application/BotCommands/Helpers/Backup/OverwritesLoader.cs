using Discord;
using Discord.WebSocket;
using Domain.Models;

namespace Application.BotCommands.Helpers.Backup;

public class OverwritesLoader(SocketGuild discordGuild)
{
    private SocketGuild DiscordGuild { get; } = discordGuild;

    // Loading channel specific permissions
    public void LoadRoleOverwrites(Category dbCategory, Channel dbChannel, IEnumerable<Overwrite> permissionOverwrites)
    {
        foreach (var overwrite in permissionOverwrites)
        {
            if (overwrite.TargetType != PermissionTarget.Role) continue;

            var roleOverwrite = new RoleOverwrite
            {
                RoleName = DiscordGuild.GetRole(overwrite.TargetId).Name,
                AllowValue = overwrite.Permissions.AllowValue,
                DenyValue = overwrite.Permissions.DenyValue,
                Category = dbCategory,
                Channel = dbChannel
            };

            dbChannel.RoleOverwrites.Add(roleOverwrite);
        }
    }

    // Loading category specific permissions
    public void LoadRoleOverwrites(Category dbCategory, IEnumerable<Overwrite> permissionOverwrites)
    {
        foreach (var overwrite in permissionOverwrites)
        {
            if (overwrite.TargetType != PermissionTarget.Role) continue;

            var roleOverwrite = new RoleOverwrite
            {
                RoleName = DiscordGuild.GetRole(overwrite.TargetId).Name,
                AllowValue = overwrite.Permissions.AllowValue,
                DenyValue = overwrite.Permissions.DenyValue,
                Category = dbCategory,
                Channel = null
            };

            dbCategory.RoleOverwrites.Add(roleOverwrite);
        }
    }
}