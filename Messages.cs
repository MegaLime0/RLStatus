using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

namespace RLStatus;

public static class Messages
{
    private static DiscordColor Color = new("#0E212D");
    private static DiscordEmbedBuilder.EmbedAuthor Author = new()
    {
        Name = "Vroom",
        IconUrl = "https://cdn.discordapp.com/avatars/1217477160340815985/f1f53c4ec2396c9dce125ab1611620d8.webp",
        Url = "https://github.com/MegaLime0/RLStatus"
    };

    private static string GetDescription(string cmdName)
    {
        string helpDescription = "Shows this menu";
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

        embed.Color = Color;
        embed.Author = Author;
        embed.Title = "**Help Menu**";
        embed.Description = "Dont spam or I'll explode";

        foreach (var cmd in cmds)
        {
            embed.AddField(
                $"</{cmd.Name}:{cmd.Id}>",
                "> " + GetDescription(cmd.Name)
            );
        }

        webhook.AddEmbed(embed);
        return webhook;
    }

    private static ButtonStyle IsSelected(StatType playlist, StatType selection)
    {
        if (playlist == selection)
        {
            return ButtonStyle.Primary;
        }
        else
        {
            return ButtonStyle.Secondary;
        }
    }

    private static DiscordWebhookBuilder AddButtonsToWebhook(DiscordWebhookBuilder webhook, ulong userId, StatType current)
    {
        webhook.AddComponents(new DiscordComponent[]
                {
                new DiscordButtonComponent(IsSelected(StatType.Overall,current), StatType.Overall.ToString() + $"_{userId}", "Overall"),
                new DiscordButtonComponent(IsSelected(StatType.Vs1,current), StatType.Vs1.ToString() + $"_{userId}", "1v1"),
                new DiscordButtonComponent(IsSelected(StatType.Vs2,current), StatType.Vs2.ToString() + $"_{userId}", "2v2"),
                new DiscordButtonComponent(IsSelected(StatType.Vs3,current), StatType.Vs3.ToString() + $"_{userId}", "3v3"),
                new DiscordButtonComponent(IsSelected(StatType.Rumble,current), StatType.Rumble.ToString() + $"_{userId}", "Rumble"),
                });
        webhook.AddComponents(new DiscordComponent[]
                {
                new DiscordButtonComponent(IsSelected(StatType.Casual,current), StatType.Casual.ToString() + $"_{userId}", "Casual"),
                new DiscordButtonComponent(IsSelected(StatType.Hoops,current), StatType.Hoops.ToString() + $"_{userId}", "Hoops"),
                new DiscordButtonComponent(IsSelected(StatType.Dropshot,current), StatType.Dropshot.ToString() + $"_{userId}", "Dropshot"),
                new DiscordButtonComponent(IsSelected(StatType.Snowday,current), StatType.Snowday.ToString() + $"_{userId}", "Snowday"),
                });

        return webhook;
    }

    private static DiscordInteractionResponseBuilder AddButtonsToInteraction(DiscordInteractionResponseBuilder interaction, ulong userId, StatType current)
    {
        interaction.AddComponents(new DiscordComponent[]
                {
                new DiscordButtonComponent(IsSelected(StatType.Overall,current), StatType.Overall.ToString() + $"_{userId}", "Overall"),
                new DiscordButtonComponent(IsSelected(StatType.Vs1,current), StatType.Vs1.ToString() + $"_{userId}", "1v1"),
                new DiscordButtonComponent(IsSelected(StatType.Vs2,current), StatType.Vs2.ToString() + $"_{userId}", "2v2"),
                new DiscordButtonComponent(IsSelected(StatType.Vs3,current), StatType.Vs3.ToString() + $"_{userId}", "3v3"),
                new DiscordButtonComponent(IsSelected(StatType.Rumble,current), StatType.Rumble.ToString() + $"_{userId}", "Rumble"),
                });
        interaction.AddComponents(new DiscordComponent[]
                {
                new DiscordButtonComponent(IsSelected(StatType.Casual,current), StatType.Casual.ToString() + $"_{userId}", "Casual"),
                new DiscordButtonComponent(IsSelected(StatType.Hoops,current), StatType.Hoops.ToString() + $"_{userId}", "Hoops"),
                new DiscordButtonComponent(IsSelected(StatType.Dropshot,current), StatType.Dropshot.ToString() + $"_{userId}", "Dropshot"),
                new DiscordButtonComponent(IsSelected(StatType.Snowday,current), StatType.Snowday.ToString() + $"_{userId}", "Snowday"),
                });

        return interaction;
    }

    public static async Task<DiscordWebhookBuilder> Stats(Database db, ulong userId, Query query, StatType selection, DiscordWebhookBuilder output)
    {
        if (selection == StatType.Overall)
        {
            DiscordWebhookBuilder webhook = new();
            var embed = await OverallStats(db, userId, query, selection);

            webhook.AddEmbed(embed);
            webhook = AddButtonsToWebhook(webhook, userId, selection);

            return webhook;
        }
        else
        {
            DiscordWebhookBuilder webhook = new();
            var embed = await ModeStats(db, userId, query, selection);

            webhook.AddEmbed(embed);
            webhook = AddButtonsToWebhook(webhook, userId, selection);

            return webhook;
        }
    }

    public static async Task<DiscordInteractionResponseBuilder> InteractionStats(Database db, ulong userId, Query query, StatType selection)
    {
        if (selection == StatType.Overall)
        {
            DiscordInteractionResponseBuilder interaction = new();
            var embed = await OverallStats(db, userId, query, selection);

            interaction.AddEmbed(embed);
            interaction = AddButtonsToInteraction(interaction, userId, selection);

            return interaction;
        }
        else
        {
            DiscordInteractionResponseBuilder interaction = new();
            var embed = await ModeStats(db, userId, query, selection);

            interaction.AddEmbed(embed);
            interaction = AddButtonsToInteraction(interaction, userId, selection);

            return interaction;
        }
    }

    private static async Task<DiscordEmbedBuilder> OverallStats(Database db, ulong userId, Query query, StatType selection)
    {
        DiscordEmbedBuilder embed = new();

        Stats? stats = await db.RetreiveStats(userId, query);

        embed.Color = Color;
        embed.Author = Author;
        embed.Title = $"Stats for {stats!.Username}";
        embed.Description = "**Overall stats**";

        embed.AddField(
                ":crown: Wins",
                $"{stats.Wins}",
                true
                );

        embed.AddField(
                ":soccer: Shots",
                $"{stats.Shots}",
                true
                );

        embed.AddField(
                ":shield: Saves",
                $"{stats.Saves}",
                true
                );

        embed.AddField(
                ":handshake: Assists",
                $"{stats.Assists}",
                true
                );

        embed.AddField(
                ":goal: Goals",
                $"{stats.Goals}",
                true
                );

        embed.AddField(
                ":star: MVPs",
                $"{stats.MVPs}",
                true
                );

        embed.AddField(
                ":gift: Reward Level",
                stats.RewardLevel.ToString().Replace("2", ""),
                true
                );

        embed.AddField(
                ":eyes: Profile Views",
                $"{stats.ProfileViews}",
                true
                );

        embed.Thumbnail = new() { Url = Loader.RankIcons.GetValueOrDefault(Ranks.Unranked) };

        return embed;
    }

    private static async Task<DiscordEmbedBuilder> ModeStats(Database db, ulong userId, Query query, StatType playlist)
    {
        DiscordEmbedBuilder embed = new();

        Stats? stats = await db.RetreiveStats(userId, query);

        embed.Color = Color;
        embed.Author = Author;
        embed.Title = $"Stats for {stats!.Username}";

        // This has got to be the worst thing ive ever written
        string playlistName = playlist.ToString();
        char lastCharacter = playlistName[playlistName.Length - 1];

        if (Char.IsDigit(lastCharacter))
        {
            playlistName = lastCharacter + playlistName;
        }

        embed.Description = $"**{playlistName}**";

        Mode mode;
        switch (playlist)
        {
            case StatType.Vs1:
                mode = stats.Vs1!;
                break;
            case StatType.Vs2:
                mode = stats.Vs2!;
                break;
            case StatType.Vs3:
                mode = stats.Vs3!;
                break;
            case StatType.Casual:
                mode = stats.Casual!;
                break;
            case StatType.Dropshot:
                mode = stats.Dropshot!;
                break;
            case StatType.Hoops:
                mode = stats.Hoops!;
                break;
            case StatType.Rumble:
                mode = stats.Rumble!;
                break;
            case StatType.Snowday:
                mode = stats.Snowday!;
                break;
            default:
                mode = new(0, 0, Ranks.Unranked, Playlists.Casual);
                Console.WriteLine("CASE DEFAULT IN MESSAGES 266");
                break;
        }


        // Same garbage strat. adds a space before the number
        string rank = mode.Rank.ToString();
        lastCharacter = rank[rank.Length - 1];
        if (Char.IsDigit(lastCharacter))
        {
            rank = rank.Replace(lastCharacter, ' ');
            rank += lastCharacter;
        }

        embed.AddField(
                ":trophy: Rank",
                rank,
                true
                );

        embed.AddField(
                ":bar_chart: MMR",
                $"{mode.MMR}",
                true
                );

        embed.AddField(
                ":ladder: Division",
                $"{mode.Division}",
                true
                );

        embed.Thumbnail = new() { Url = Loader.RankIcons.GetValueOrDefault(mode.Rank) };
        // embed.ImageUrl = Loader.RankIcons.GetValueOrDefault(mode.Rank);

        return embed;
    }

    public static DiscordWebhookBuilder AccountExists()
    {
        DiscordWebhookBuilder webhook = new();
        DiscordEmbedBuilder embed = new();

        embed.Color = Color;
        embed.Author = Author;
        embed.Title = "** Your Account is Already Set Up**";

        embed.Description = "To Check Your Stats Use **/setup**";

        webhook.AddEmbed(embed);
        return webhook;
    }

    public static DiscordWebhookBuilder DatabaseAccountNotFound()
    {
        DiscordWebhookBuilder webhook = new();
        DiscordEmbedBuilder embed = new();

        embed.Color = Color;
        embed.Author = Author;
        embed.Title = "** Account not set up **";

        embed.Description = "To set up you account use **/setup**";

        webhook.AddEmbed(embed);
        return webhook;
    }

    public static DiscordWebhookBuilder StatAccountNotFound(string attemptedUsername, Platforms platform)
    {
        DiscordWebhookBuilder webhook = new();
        DiscordEmbedBuilder embed = new();

        embed.Color = Color;
        embed.Author = Author;
        embed.Title = "** Username not found **";

        embed.Description = $"Stats for **{attemptedUsername}** on **{platform.ToString()}** not found."
                            + "\nMake sure you spelled it correctly and used the correct platform";

        webhook.AddEmbed(embed);
        return webhook;
    }
}
