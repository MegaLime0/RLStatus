using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

namespace RLStatus;

public class SlashCommands : ApplicationCommandModule
{
    static DiscordClient? client;
    static Database db = Database.Instance;
    static Query query = Query.Instance;

    public static void StoreClient(DiscordClient _client)
    {
        client = _client;
    }

    [SlashCommand("setup", "Set your RL account")]
    public async Task SetAcc(InteractionContext ctx,
            [Option("Username", "RL account name")] string username,
            [Option("Platform", "Select your RL platform")] Platforms platform = Platforms.Epic)
    {
        Console.WriteLine($"Command: setup, Platform: {platform}");

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        if (db.AccountExists(ctx.User.Id))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Your account is already set up"));
            return;
        }

        if (!await query.AccountExists(platform, username))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Couldnt find username"));
            return;
        }

        db.SaveAccount(username, ctx.User.Id, platform);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Account successfully set up"));
    }

    [SlashCommand("stats", "Get your RL stats")]
    public async Task Stats(InteractionContext ctx,
            [Option("Username", "RL account name")] string username = "")
    {
        // TODO: This should only work if the user has set up an 
        // account, if they dont have one, send out a prompt to
        // set it up
        if (username == "")
        {
            // TODO: Add logic to find rl username associated to
            //       this discord account and get stat data
        }

        // TODO: Add logic to get stats for rl username

        // TODO: Change this response logic to wait a bit for the stats
        // to get parsed and stored in the database
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
