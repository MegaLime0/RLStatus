using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

namespace RLStatus;

public static class Messages
{
    private static DiscordColor EmbedColor = new("#0E212D");

    private static string GetDescription(string cmdName)
    {
        string helpDescription = "Shows this command";
        string statsDescription = "Displays your rocket league stats";
        string setupDescription = "Link your rocket league and discord accounts together";

        switch (cmdName.ToLower())
        {
            case "setup":
                return setupDescription;
            case "help":
                return helpDescription;
            case "stats":
                return statsDescription;
            default:
                Console.WriteLine($"No Description for {cmdName}");
                return "No Description";
        }
    }

    public static DiscordWebhookBuilder Help(SlashCommandsExtension slash)
    {
        var cmds = slash.RegisteredCommands[0].Value;
        DiscordWebhookBuilder webhook = new();
        DiscordEmbedBuilder embed = new();

        embed.Color = EmbedColor;
        embed.Title = "AVAILABLE COMMANDS";
        embed.Description = "Dont spam or ill explode";

        foreach (var cmd in cmds)
        {
            embed.AddField(
                $"</{cmd.Name}:{cmd.Id}>",
                GetDescription(cmd.Name)
            );
        }

        webhook.AddEmbed(embed);
        return webhook;
    }

    public static async Task<DiscordWebhookBuilder?> OverallStats(Database db, ulong userId, Query query)
    {
        DiscordWebhookBuilder webhook = new();
        DiscordEmbedBuilder embed = new();

        Stats? stats = await db.RetreiveStats(userId, query);

        embed.Color = EmbedColor;
        embed.Title = $"Stats for {stats!.Username}";
        embed.Description = "Overall stats";
        // TODO: Send Overall Stats embed
        return null;
    }

    public static async Task<DiscordWebhookBuilder?> ModeStats(Database db, ulong userId, Query query, StatType type)
    {
        DiscordWebhookBuilder webhook = new();
        DiscordEmbedBuilder embed = new();

        Stats? stats = await db.RetreiveStats(userId, query);

        // TODO: Send specific mode stats embed
        return null;
    }

    public static DiscordWebhookBuilder AccountExists(ulong userId)
    {
        // TODO
        return new();
    }

    public static DiscordWebhookBuilder DatabaseAccountNotFound(ulong userId)
    {
        // TODO
        return new();
    }

    public static DiscordWebhookBuilder StatAccountNotFound(ulong userId)
    {
        // TODO
        return new();
    }
}
