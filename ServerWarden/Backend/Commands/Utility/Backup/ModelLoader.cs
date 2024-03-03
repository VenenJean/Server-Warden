using Backend.Persistence;
using Backend.Persistence.Models;

namespace Backend.Commands.Utility.Backup;

public class ModelLoader {
  /* States
   * filled => ok
   * both / one is new() => throw error
   * null => throw error
   */
  public static List<string>? ChannelsForMessages { get; set; } = new();
  public static List<string>? RolesForUsers { get; set; } = new();

  private readonly SocketGuild _discordGuild;
  public readonly Guild GuildModel;

  public ModelLoader(SocketGuild guild) {
    _discordGuild = guild;
    GuildModel = new Guild {
      Name = _discordGuild.Name,
      Description = _discordGuild.Description ?? string.Empty
    };
  }
  
  public async Task<bool> LoadDataAsync() {
    await FillGuildModelAsync();
    
    if (ChannelsForMessages == null || RolesForUsers == null) return false;
    if (ChannelsForMessages.Count == 0 || RolesForUsers.Count == 0) return false;
    
    await using var context = new BackupContext();
    await context.Database.EnsureCreatedAsync();

    await context.AddAsync(GuildModel);
    await context.AddRangeAsync(GuildModel.Roles);
    await context.AddRangeAsync(GuildModel.Categories);
    
    foreach (var category in GuildModel.Categories) {
      await context.AddRangeAsync(category.Channels);
        
      foreach (var channel in category.Channels) {
        await context.AddRangeAsync(channel.Messages);
        await context.AddRangeAsync(channel.RoleOverwrites);
      }
        
      await context.AddRangeAsync(category.RoleOverwrites);
    }

    await context.SaveChangesAsync();
    return true;
  }
  
  private async Task FillGuildModelAsync() {
    LoadRoles();
    await LoadCategoriesAsync();
  }
  
  private void LoadRoles() {
    foreach (var role in _discordGuild.Roles) {
      if (role.IsManaged) continue; 
      
      Role roleObj;
      if (!role.IsEveryone) {
        roleObj = new Role {
          Name = role.Name,
          Color = role.Color.RawValue,
          Position = role.Position,
          IsHoisted = role.IsHoisted,
          IsMentionable = role.IsMentionable,
          BitwisePermissionFlag = role.Permissions.RawValue,
          Guild = GuildModel
        };
      } else { 
        roleObj = new Role {
          Name = "@everyone",
          BitwisePermissionFlag = role.Permissions.RawValue
        };
      }
      
      GuildModel.Roles.Add(roleObj);
    }
  }
  
  private async Task LoadCategoriesAsync() {
    foreach (var category in _discordGuild.CategoryChannels) {
      var categoryObj = new Category {
        Name = category.Name,
        Position = category.Position,
        Guild = GuildModel
      };

      LoadRoleOverwrites(categoryObj, category.PermissionOverwrites);
      await LoadChannelsAsync(categoryObj, category);
        
      GuildModel.Categories.Add(categoryObj);
    }
  }
  
  private async Task LoadChannelsAsync(Category dbCategory, SocketCategoryChannel discordCategory) {
    foreach (var channel in discordCategory.Channels) {
      var channelObj = new Channel {
        Name = channel.Name,
        Position = channel.Position,
        Type = channel.GetChannelType().Value.ToString(),
        Category = dbCategory
      };

      await LoadMessagesAsync(channelObj, channel);
      LoadRoleOverwrites(dbCategory, channelObj, channel.PermissionOverwrites);
          
      dbCategory.Channels.Add(channelObj);
    }
  }
  
  private async Task LoadMessagesAsync(Channel dbChannel, SocketGuildChannel discordChannel) {
    if (ChannelsForMessages != null && ChannelsForMessages.Contains(discordChannel.Name) && discordChannel is ITextChannel textChannel) {
      var messages = await textChannel.GetMessagesAsync().FlattenAsync();

      foreach (var message in messages) {
        const int maxMsgLength = 2000;
        if (message.Content == string.Empty || message.Content.Length > maxMsgLength) continue;
        var messageObj = new Message {
          Content = message.Content,
          SentTime = message.Timestamp
        };
        dbChannel.Messages.Add(messageObj);
      }

      dbChannel.Messages.Reverse();
    }
  }
  
  // Loading channel specific permissions
  private void LoadRoleOverwrites(Category dbCategory, Channel dbChannel, IEnumerable<Overwrite> permissionOverwrites) {
    foreach (var overwrite in permissionOverwrites) {
      if (overwrite.TargetType != PermissionTarget.Role) continue;

      var roleOverwrite = new RoleOverwrite {
        RoleName = _discordGuild.GetRole(overwrite.TargetId).Name,
        AllowValue = overwrite.Permissions.AllowValue,
        DenyValue = overwrite.Permissions.DenyValue,
        Category = dbCategory,
        Channel = dbChannel
      };
          
      dbChannel.RoleOverwrites.Add(roleOverwrite);
    }
  }
  
  // Loading category specific permissions
  private void LoadRoleOverwrites(Category dbCategory, IEnumerable<Overwrite> permissionOverwrites) {
    foreach (var overwrite in permissionOverwrites) {
      if (overwrite.TargetType != PermissionTarget.Role) continue;

      var roleOverwrite = new RoleOverwrite {
        RoleName = _discordGuild.GetRole(overwrite.TargetId).Name,
        AllowValue = overwrite.Permissions.AllowValue,
        DenyValue = overwrite.Permissions.DenyValue,
        Category = dbCategory,
        Channel = null
      };
          
      dbCategory.RoleOverwrites.Add(roleOverwrite);
    }
  }
}