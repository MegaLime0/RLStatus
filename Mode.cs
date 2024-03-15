using System.Text.Json;

namespace RLStatus;

public class Mode
{
    public uint MMR { get; private set; }
    public uint Division { get; private set; }
    public Ranks Rank { get; private set; }

    public Mode(JsonElement stats)
    {
        stats = stats.GetProperty("stats");
        MMR = stats.GetProperty("rating")
            .GetProperty("value")
            .GetUInt32();

        Rank = (Ranks) ((stats.GetProperty("tier").GetProperty("value").GetInt16() - 1) / 3);

        Division = stats.GetProperty("division").GetProperty("value").GetUInt16() + (uint) 1;
    }
}
