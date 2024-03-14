namespace RLStatus;

public static class Query
{
     private static string _rl_api = "https://api.tracker.gg/api/v5/rocket-league/standard/profile/";
     private static string _steam_api = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/"
                                        + $"?key={Loader.SteamWebAPI}&vanityurl=";
}
