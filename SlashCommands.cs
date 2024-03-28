using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

namespace RLStatus;

public class SlashCommands : ApplicationCommandModule
{
    static SlashCommandsExtension? slash;
    static Database db = Database.Instance;
    static Query query = Query.Instance;

    public static void SetSlashCommandExtension(SlashCommandsExtension _slash)
    {
        slash = _slash;
    }

    [SlashCommand("setup", "Set your RL account")]
    public async Task SetAcc(InteractionContext ctx,
        [Option("Username", "RL account name")] string username,
        [Option("Platform", "Select your RL platform")] Platforms platform = Platforms.Epic)
    {
        Console.WriteLine($"Command: setup, Platform: {platform}");

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        // Checks database for account
        if (db.AccountExists(ctx.User.Id))
        {
            // TODO: Replace with Messages.AccountExists()
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Your account is already set up"));
            return;
        }

        // Checks rl stats for acocunt
        if (!await query.AccountExists(platform, username))
        {
            // TODO: Replace with Messages.StatAccountNotFound()
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Couldnt find username"));
            return;
        }

        db.SaveAccount(username, ctx.User.Id, platform);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Account successfully set up"));
    }

    [SlashCommand("stats", "Get your RL stats")]
    public async Task Stats(InteractionContext ctx,
        [Option("StatType", "Overall stats or stats for a specific mode")] StatType statType = StatType.Overall)
    {
        if (!db.AccountExists(ctx.User.Id))
        {
            // TODO: Send Messages.DatabaseAccountNotFound()
            return;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Stats for {ctx.User}")); // Temporary

        Stats? stats = await db.RetreiveStats(ctx.User.Id, query);

        // TODO: Send Messages.OverallStats() or Messages.ModeStats()
    }

    [SlashCommand("help", "Usage instructions")]
    public async Task Help(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        DiscordWebhookBuilder webhook = Messages.Help(slash!);
        await ctx.EditResponseAsync(webhook);
    }
}
