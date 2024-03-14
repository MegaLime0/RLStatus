using System.Text.Json.Nodes;

namespace RLStatus;

public sealed class Query
{
    private static Query? instance = null;
    private const string _rl_api = "https://api.tracker.gg/api/v5/rocket-league/standard/profile/";
    private static string _steam_api = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/"
                                        + $"?key={Loader.SteamWebAPI()}&vanityurl=";

    private HttpClient client = new();

    private Query()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add(
                "Accept",
                "application/json"
                );
        client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
                );
    }

    public static Query Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new();
            }
            return instance;
        }
    }

    public async Task<long> SteamUserId(string vanityUrl)
    {
        string url = _steam_api + vanityUrl;
        HttpResponseMessage response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.StatusCode);
            return 0;
        }

        JsonNode output = JsonNode.Parse(await response.Content.ReadAsStringAsync())!["response"]!;
        long userId = Convert.ToInt64(output["steamid"]!.ToString());
        
        Console.WriteLine($"Queried: {vanityUrl}, userId: {userId}");
        Console.WriteLine(output);
        return 0;
    }
}
