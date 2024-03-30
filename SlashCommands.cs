using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

namespace RLStatus;

public class SlashCommands : ApplicationCommandModule
{
    static Database db = Database.Instance;
    static Query query = Query.Instance;

    [SlashCommand("setup", "Set your RL account")]
    public async Task SetAcc(InteractionContext ctx,
        [Option("Username", "RL account name")] string username,
        [Option("Platform", "Select your RL platform")] Platforms platform = Platforms.Epic)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        // Checks database for account
        if (db.AccountExists(ctx.User.Id))
        {
            await ctx.EditResponseAsync(Messages.AccountExists());
            return;
        }

        // Checks rl stats for acocunt
        if (!await query.AccountExists(platform, username))
        {
            await ctx.EditResponseAsync(Messages.StatAccountNotFound(username, platform));
            return;
        }

        db.SaveAccount(username, ctx.User.Id, platform);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Account successfully set up"));
    }

    [SlashCommand("stats", "Get your RL stats")]
    public async Task Stats(InteractionContext ctx,
        [Option("Stat", "Overall stats or stats for a specific mode")] StatType statType = StatType.Overall)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        if (!db.AccountExists(ctx.User.Id))
        {
            await ctx.EditResponseAsync(Messages.DatabaseAccountNotFound());
            return;
        }

        DiscordWebhookBuilder webhook = new();
        webhook = await Messages.Stats(db, ctx.User.Id, query, statType, webhook);
        await ctx.EditResponseAsync(webhook);
    }

    [SlashCommand("help", "Usage instructions")]
    public async Task Help(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        DiscordWebhookBuilder webhook = Messages.Help(ctx.Client.GetSlashCommands());
        await ctx.EditResponseAsync(webhook);
    }
}
