using Discord;

namespace Application.Listeners;

public class MessageListener
{
    public async Task HandleMessageDeleted(Cacheable<IMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel)
    {
        var deletedMessage = await message.GetOrDownloadAsync();
        const ulong logChannelId = 1257055718650155028;
        var logChannel = await DiscordFactory.GetClient().GetChannelAsync(logChannelId) as IMessageChannel;

        if (logChannel != null)
        {
            try
            {
                await logChannel.SendMessageAsync($"{deletedMessage.Channel.Name} | {deletedMessage.Author.GlobalName}: {deletedMessage.Content}");
            }
            catch (NullReferenceException e)
            {
                
            }
        }
    }
}