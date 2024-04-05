using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace RLStatus;

public static class Program
{
    static async Task Main()
    {
        Loader.Initialize(); // Always run first
        // ERROR HANDLING TODO: Everywhere where a variable is nullable
        // add a check to make sure it isnt null
        // use try catch statements more
        await StartBot();
        await Task.Delay(-1);
    }

    static async Task StartBot()
    {
        string _token = Loader.DiscordToken;
        DiscordConfiguration conf =
            new()
            {
                Token = _token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
            };

        DiscordClient client = new(conf);

        var slashCmds = client.UseSlashCommands();
        slashCmds.RegisterCommands<SlashCommands>();

        slashCmds.SlashCommandErrored += EventHandlers.OnSlashCommandError;
        slashCmds.ContextMenuErrored += EventHandlers.OnContextMenuError;
        slashCmds.AutocompleteErrored += EventHandlers.OnAutocompleteError;

        slashCmds.SlashCommandInvoked += EventHandlers.OnSlashCommandInvoke;

        client.Ready += EventHandlers.OnReady;
        client.MessageCreated += EventHandlers.OnMessage;
        client.ComponentInteractionCreated += EventHandlers.OnButtonClick;

        await client.ConnectAsync();
    }
}
