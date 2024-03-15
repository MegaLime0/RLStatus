using DSharpPlus.SlashCommands;

namespace RLStatus;

public enum Platforms
{
    [ChoiceName("Steam")]
    Steam,
    [ChoiceName("Epic Games")]
    EpicGames,
    [ChoiceName("Xbox")]
    Xbox,
    [ChoiceName("PlayStation")]
    PlayStation,
    [ChoiceName("Nintendo Switch")]
    NintendoSwitch,
}
