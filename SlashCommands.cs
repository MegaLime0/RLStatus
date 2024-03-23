using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

namespace RLStatus;

public class SlashCommands : ApplicationCommandModule 
{
    static DiscordClient? client;
    public static void StoreClient(DiscordClient _client)
    {
        client = _client;
    }

    [SlashCommand("setup", "Set your RL account")]
    public async Task SetAcc(InteractionContext ctx,
            [Option("Username", "RL account name")] string username,
            [Option("Platform", "Select your RL platform")] Platforms platform = Platforms.Epic)
    {
        Console.WriteLine($"Command: setacc, Platform: {platform}");

        // TODO: Add logic to find and store info in a database
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
             new DiscordInteractionResponseBuilder().WithContent("Works"));
    }

    [SlashCommand("stats", "Get your RL stats")]
    public async Task Stats(InteractionContext ctx,
            [Option("Username", "RL account name")] string username = "")
    {
        if (username == "")
        {
            // TODO: Add logic to find rl username associated to
            //       this discord account and get stat data
        }

        // TODO: Add logic to get stats for rl username

        // TODO: Change this response logic to wait a bit for the stats
        // to get parsed and stored in the database

        Query q = Query.Instance;
        long balls = await q.SteamUserId("https://steamcommunity.com/id/MegaLime0");
        Console.WriteLine($"Requested Context by {ctx.User.Username}");
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Works"));
    }

    [SlashCommand("help", "Usage instructions")]
    public async Task Help(InteractionContext ctx)
    {
        // TODO: Add help string
        
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        // IReadOnlyList<DiscordApplicationCommand> cmds = await ctx.Guild.GetApplicationCommandsAsync();
        IReadOnlyList<DiscordApplicationCommand> cmds = await client!.GetGlobalApplicationCommandsAsync();
        var builder = new DiscordWebhookBuilder().WithContent("Commands List\n");
        foreach (DiscordApplicationCommand c in cmds)
        {
            builder.Content += $"</{c.Name}:{c.Id}>";
        }
        await ctx.EditResponseAsync(builder);
    }
}
