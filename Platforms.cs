using DSharpPlus.SlashCommands;

namespace RLStatus;

public enum Platforms
{
    [ChoiceName("Steam")]
    Steam,

    [ChoiceName("Epic Games")]
    Epic,

    [ChoiceName("Xbox")]
    XBL,

    [ChoiceName("PlayStation")]
    PSn,

    [ChoiceName("Nintendo Switch")]
    Switch,
}
