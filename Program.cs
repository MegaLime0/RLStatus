using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace RLStatus;

public class Program
{
    static async Task Main()
    {
        await StartBot();
        await Task.Delay(-1);

    }

    static async Task StartBot()
    {
        string _token = Loader.DiscordToken();
        DiscordConfiguration conf = new() 
        {
            Token = _token,
                  TokenType = TokenType.Bot,
                  Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
        };

        DiscordClient client = new(conf);

        var slashCMDS = client.UseSlashCommands();

        slashCMDS.RegisterCommands<SlashCommands>();
        SlashCommands.StoreClient(client);

        await client.ConnectAsync();

        client.Ready += EventHandlers.OnReady;
        client.MessageCreated += EventHandlers.OnMessage;
    }
}
