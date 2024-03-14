using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace RLStatus;

public class Program
{
    static string GetToken(string TokenName = ".discord_token")
    {
        return File.ReadAllText(TokenName);
    }

    static async Task Main()
    {
        string _token = GetToken();
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

        await Task.Delay(-1);
    }
}
