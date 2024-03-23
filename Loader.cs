namespace RLStatus;

public static class Loader
{
    public static string DiscordToken(string path = "tokens.txt")
    {
        return File.ReadAllLines(path)[0].Trim();
    }

    public static string SteamWebAPI(string path = "tokens.txt")
    {
        return File.ReadAllLines(path)[1].Trim();
    }
}
