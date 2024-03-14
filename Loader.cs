namespace RLStatus;

public static class Loader
{
    public static string DiscordToken(string path = ".discord_token")
    {
        return File.ReadAllText(path);
    }

    public static string SteamWebAPI(string path = ".steam_api")
    {
        return File.ReadAllText(path);
    }
}
