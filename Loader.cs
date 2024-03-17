namespace RLStatus;

public static class Loader
{
    public static string DiscordToken(string path = ".discord_token")
    {
        return File.ReadAllLines(path)[0].Trim();
    }

    public static string SteamWebAPI(string path = ".steam_api")
    {
        return File.ReadAllLines(path)[1].Trim();
    }
}
