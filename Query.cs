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
        var (result, type) = Extractor.SteamUrl(vanityUrl);

        if (type == UrlType.Id)
        {
            Console.WriteLine($"Id From Direct Link: {result}");
            return Convert.ToInt64(result);
        }
        else
        {
            string url = _steam_api + result;
            string response = await GetPageString(url);

            JsonNode output = JsonNode.Parse(response)!["response"]!; 
            int success = (int)output["success"]!;

            if (success != 1)
            {
                Console.WriteLine($"Steam WebApi Failed For: {result}");
                return -1;
            }

            string userId = (string)output["steamid"]!;

            Console.WriteLine($"Queried: {result}, userId: {userId}");
            Console.WriteLine(output);

            return Convert.ToInt64(userId);
        }
    }

    private async Task<string> GetPageString(string url)
    {
        HttpResponseMessage response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return "Error";
        }

        return (await response.Content.ReadAsStringAsync());
    }
}
