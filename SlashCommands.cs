using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

namespace RLStatus;

public class SlashCommands : ApplicationCommandModule 
{
    [SlashCommand("setacc", "Set your RL account")]
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

        Console.WriteLine($"Requested Context by {ctx.User.Username}");
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Works"));
    }

    [SlashCommand("help", "Usage instructions")]
    public async Task Help(InteractionContext ctx)
    {
        // TODO: Add help string
    }
}
