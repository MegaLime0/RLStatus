using DSharpPlus;
using DSharpPlus.EventArgs;

namespace RLStatus;

static public class EventHandlers
{
    static public Task OnReady(DiscordClient client, ReadyEventArgs args)
    {
        Console.WriteLine(
                $"Client {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}" 
                + " Successfully Connected");
        return Task.CompletedTask;
    }

    static public async Task OnMessage(DiscordClient client, MessageCreateEventArgs args)
    {
        if (args.Message.Content == "\\summon tnt") 
        {
            await args.Channel.SendMessageAsync("T-T");
        }
    }
}
