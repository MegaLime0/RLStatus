using System.Text.Json;

namespace RLStatus;

public static class Loader
{
    public static Dictionary<Ranks, string> RankIcons = new();
    public static string DiscordToken = string.Empty;
    public static string SteamWebToken = string.Empty;

    public static void Initialize(string path = "config.json")
    {
        using (JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path)))
        {
            JsonElement root = doc.RootElement;
            DiscordToken = root.GetProperty("DiscordToken").GetString()!;
            SteamWebToken = root.GetProperty("SteamWebToken").GetString()!;
            JsonElement iconUrls = root.GetProperty("IconUrls");
            foreach (Ranks rank in Enum.GetValues(typeof(Ranks)))
            {
                RankIcons.Add(rank, iconUrls.GetProperty(rank.ToString()).GetString()!);
            }
        }
    }
}
