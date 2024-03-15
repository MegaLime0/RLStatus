using System.Text.RegularExpressions;

namespace RLStatus;

public static class Extractor
{
    private static Regex namePatt = new(@"https:\/\/steamcommunity\.com\/id\/(\w*)\/?");
    private static Regex idPatt = new(@"https:\/\/steamcommunity\.com\/profiles\/(\d+)");

    private static string GetVanityName(string profileUrl)
    {
        Match m = namePatt.Match(profileUrl);
        if (!m.Success)
        {
            Console.WriteLine("Url Failed");
            return "";
        }

        return m.Groups[1].Value;
    }

    private static string GetUserId(string profileUrl)
    {
        Match m = idPatt.Match(profileUrl);
        if (!m.Success)
        {
            Console.WriteLine("Profile Failed");
            return "";
        }

        return m.Groups[1].Value;
    }

    public static (string, UrlType) SteamUrl(string profileUrl)
    {
        if (profileUrl.Contains("id"))
        {
            return (GetVanityName(profileUrl), UrlType.Name);
        }
        else
        {
            return (GetUserId(profileUrl), UrlType.Id);
        }
    }
}
