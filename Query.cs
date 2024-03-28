using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace RLStatus;

public sealed class Query : IDisposable
{
    private static Query? instance = null;
    private static Object lockObject = new();
    private const string _rl_api = "https://api.tracker.gg/api/v2/rocket-league/standard/profile/";
    private static string _steam_api = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/"
                                        + $"?key={Loader.SteamWebToken}&vanityurl=";

    private static WebDriver? driver;
    private static FirefoxOptions? opts;
    private static FirefoxDriverService? srv;

    private HttpClient client = new();

    private Query()
    {
        client.DefaultRequestHeaders.Add(
                "Accept",
                "application/json"
                );
        client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
                );

        Environment.SetEnvironmentVariable("MOZ_REMOTE_SETTINGS_DEVTOOLS", "1");

        opts = new();
        opts.AddArgument("--headless");
        opts.AddArgument("--safe-mode");
        opts.SetPreference("browser.console.loglevel", "none");
        opts.SetPreference("devtools.jsonview.enabled", false);
        opts.SetPreference("browser.contentblocking.enabled", false);
        opts.LogLevel = FirefoxDriverLogLevel.Fatal;

        srv = FirefoxDriverService.CreateDefaultService();
        srv.SuppressInitialDiagnosticInformation = true;
        srv.LogLevel = FirefoxDriverLogLevel.Fatal;

        driver = new FirefoxDriver(srv, opts);
    }

    public void Dispose()
    {
        driver!.Close();
        driver.Dispose();
        srv!.Dispose();
        driver.Quit();
        client.Dispose();
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

    private async Task<string> QueryVanityName(string vanityName)
    {
        string url = _steam_api + vanityName;
        string response = await GetPageString(url);
        Console.WriteLine("Time to query");
        string userId;
        JsonElement output;
        using (JsonDocument dez = JsonDocument.Parse(response))
        {
            output = dez
                .RootElement
                .GetProperty("response");


            int success = output.GetProperty("success").GetInt32();

            if (success != 1)
            {
                Console.WriteLine($"Steam WebApi Failed For: {vanityName}");
                return "Failed";
            }

            userId = output.GetProperty("steamid").GetString()!;
        }

        Console.WriteLine($"Queried: {vanityName}, userId: {userId}");
        Console.WriteLine(output);

        return userId;
    }

    public async Task<long> SteamUserId(string vanityUrl)
    {
        var (result, type) = Extractor.SteamUrl(vanityUrl);
        Console.WriteLine("Past extractor");
        if (result == "Failed")
        {
            return -1;
        }
        else if (type == UrlType.Id)
        {
            Console.WriteLine($"Id From Direct Link: {result}");
            return Convert.ToInt64(result);
        }
        else
        {
            string possibleId = await QueryVanityName(result);
            if (possibleId == "Failed")
            {
                return -1;
            }

            return Convert.ToInt64(possibleId);
        }
    }

    public async Task<bool> AccountExists(Platforms platform, string identifier)
    {
        string url = _rl_api + platform.ToString().ToLower() + "/" + identifier;
        Console.WriteLine(url);
        string response = await GetPageString(url, true);
        if (response == "Error")
        {
            Console.WriteLine($"Error trying to query {identifier}");
            return false;
        }

        Console.WriteLine($"got response for {identifier}");
        File.WriteAllText("Debug.json", response);

        // If the response is an error
        if (Parser.StatErrors(response))
        {
            return false;
        }

        return true;
    }

    public async Task<Stats?> GetStats(Platforms platform, string identifier, bool currentPage = false)
    {
        if (currentPage)
        {
            string page = GetCurrentPageSource();
            return Parser.GetStats(page)!;
        }

        string url = _rl_api + platform.ToString().ToLower() + "/" + identifier;
        Console.WriteLine(url);
        string response = await GetPageString(url, true);
        if (response == "Error")
        {
            Console.WriteLine($"Error trying to query {identifier}");
            return null;
        }

        Console.WriteLine($"got response for {identifier}");
        File.WriteAllText("Debug.json", response);

        Stats? stats = Parser.GetStats(response);
        if (stats == null)
        {
            Console.WriteLine("Stats returned null");
            return null;
        }
        return stats;
    }

    private async Task<string> GetPageString(string url, bool isStats = false)
    {
        if (isStats)
        {
            string src;
            lock (lockObject)
            {
                driver!.Navigate().GoToUrl(url);

                src = driver.FindElement(By.TagName("pre")).Text;
            }

            if (String.IsNullOrEmpty(src))
            {
                return "Error";
            }

            return src;
        }

        HttpResponseMessage response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.StatusCode);
            return "Error";
        }

        return (await response.Content.ReadAsStringAsync());
    }

    private string GetCurrentPageSource()
    {
        lock(lockObject)
        {
            return driver!.PageSource;
        }
    }
}
