using DSharpPlus;
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
}
