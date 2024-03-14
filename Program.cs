using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace RLStatus;

public class Program
{
    static async Task Main()
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

        slashCMDS.RegisterCommands<SlashCommands>(940320639498223677);

        await client.ConnectAsync();


        client.Ready += EventHandlers.OnReady;
        client.MessageCreated += EventHandlers.OnMessage;

        Query q = Query.Instance;
        await q.SteamUserId("megalime0");
        await Task.Delay(-1);
    }
}
