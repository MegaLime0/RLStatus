using System.Text.RegularExpressions;

namespace RLStatus;

public static class Extractor
{
    private static Regex namePatt = new(@"https:\/\/steamcommunity\.com\/id\/(\w*)\/?");
    private static Regex idPatt = new(@"https:\/\/steamcommunity\.com\/profiles\/(\d+)");
    private static Regex directId = new(@"\d{17}");

    private static string GetVanityName(string profileUrl)
    {
        Match m = namePatt.Match(profileUrl);
        if (!m.Success)
        {
            Console.WriteLine("Url Failed");
            return "Failed";
        }

        return m.Groups[1].Value;
    }

    private static string GetUserId(string profileUrl)
    {
        Match m = idPatt.Match(profileUrl);
        if (!m.Success)
        {
            Console.WriteLine("Profile Failed");
            return "Failed";
        }

        return m.Groups[1].Value;
    }

    private static string GetDirectId(string possibleId)
    {
        Match m = directId.Match(possibleId.Trim());
        if (!m.Success)
        {
            Console.WriteLine("DirectID Failed");
            return "Failed";
        }

        return m.Groups[0].Value;
    }

    public static (string, UrlType) SteamUrl(string profileUrl)
    {
        if (profileUrl.Contains("id"))
        {
            return (GetVanityName(profileUrl), UrlType.Name);
        }
        else if (profileUrl.Contains("profiles"))
        {
            return (GetUserId(profileUrl), UrlType.Id);
        }
        else
        {
            return (GetDirectId(profileUrl), UrlType.Id);
        }
    }
}
