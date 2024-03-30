using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

namespace RLStatus;

static public class EventHandlers
{
    static public Task OnReady(DiscordClient client, ReadyEventArgs args)
    {
        Console.WriteLine(
                $"Client {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}"
                + " Successfully Connected");
        return Task.CompletedTask;
    }

    static public async Task OnMessage(DiscordClient client, MessageCreateEventArgs args)
    {
        if (args.Message.Content == "\\summon tnt")
        {
            await args.Channel.SendMessageAsync("T-T");
        }
    }

    // ---------- DEBUGGING ----------
    static public Task OnSlashCommandError(SlashCommandsExtension slash, SlashCommandErrorEventArgs args)
    {
        Console.WriteLine($"Slash Command Errored: {args.Exception.Message}");
        return Task.CompletedTask;
    }

    static public Task OnContextMenuError(SlashCommandsExtension slash, ContextMenuErrorEventArgs args)
    {
        Console.WriteLine($"Slash Command Errored: {args.Exception.Message}");
        return Task.CompletedTask;
    }

    static public Task OnAutocompleteError(SlashCommandsExtension slash, AutocompleteErrorEventArgs args)
    {
        Console.WriteLine($"Slash Command Errored: {args.Exception.Message}");
        return Task.CompletedTask;
    }

    static public Task OnSlashCommandInvoke(SlashCommandsExtension slash, SlashCommandInvokedEventArgs args)
    {
        Console.WriteLine($"{args.Context.CommandName}");
        return Task.CompletedTask;
    }
    // ------- DEBUGGING END ----------

    static public async Task OnButtonClick(DiscordClient client, ComponentInteractionCreateEventArgs args)
    {
        string[] split = args.Id.Split("_");
        Enum.TryParse(split[0], out StatType statType);
        ulong statOwnerId = Convert.ToUInt64(split[1]);

        if (statOwnerId != args.User.Id)
        {
            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                    .WithContent("This isnt your menu bro"));
            return;
        }

        var response = await Messages.InteractionStats(Database.Instance, statOwnerId, Query.Instance, statType);
        await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, response);

        return;
        // Temp
        // await args.Interaction.CreateResponseAsync(
        //         InteractionResponseType.UpdateMessage,
        //         new DiscordInteractionResponseBuilder()
        //             .WithContent("BUTTONS BEGONE!")
        //         );
    }
}
