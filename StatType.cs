using DSharpPlus.SlashCommands;

namespace RLStatus;

public enum StatType
{
    [ChoiceName("Casual")]
    Casual,

    [ChoiceName("1v1")]
    Vs1,

    [ChoiceName("2v2")]
    Vs2,

    [ChoiceName("3v3")]
    Vs3,

    [ChoiceName("Hoops")]
    Hoops,

    [ChoiceName("Rumble")]
    Rumble,

    [ChoiceName("Dropshot")]
    Dropshot,

    [ChoiceName("Snowday")]
    Snowday,

    [ChoiceName("Overall Stats")]
    Overall,
}
