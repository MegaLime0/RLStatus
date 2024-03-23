using System.Text.Json;

namespace RLStatus;

public static class Parser
{
    public static Date? GetDate(string input)
    { 
        ushort year, month, day, hour, minute;
        year = Convert.ToUInt16(input.Substring(0, 4));
        month = Convert.ToUInt16(input.Substring(5, 2));
        day = Convert.ToUInt16(input.Substring(8, 2));
        hour = Convert.ToUInt16(input.Substring(11, 2));
        minute = Convert.ToUInt16(input.Substring(14, 2));
        Date output = new(year, month, day, hour, minute);
        return output;
    }

    private static Mode GetMode(JsonElement stats, Playlists playlist)
    {
        uint mmr = stats.GetProperty("rating").GetProperty("value").GetUInt32();
        uint div = stats.GetProperty("division").GetProperty("value").GetUInt16() + (uint) 1;
        Ranks rnk = (Ranks) (stats.GetProperty("tier").GetProperty("value").GetInt16() - 1);
        Mode output = new(mmr, div, rnk, playlist);

        return output;
    }

    private static Dictionary<GeneralStatTypes, uint> GetGenerics(JsonElement statsSeg, uint profViews)
    {
        Dictionary<GeneralStatTypes, uint> generics = new();
        generics.Add(GeneralStatTypes.Wins, statsSeg.GetProperty("wins").GetProperty("value").GetUInt32());
        generics.Add(GeneralStatTypes.Goals, statsSeg.GetProperty("goals").GetProperty("value").GetUInt32());
        generics.Add(GeneralStatTypes.Saves, statsSeg.GetProperty("saves").GetProperty("value").GetUInt32());
        generics.Add(GeneralStatTypes.Assists, statsSeg.GetProperty("assists").GetProperty("value").GetUInt32());
        generics.Add(GeneralStatTypes.MVPs, statsSeg.GetProperty("mVPs").GetProperty("value").GetUInt32());
        generics.Add(GeneralStatTypes.Shots, statsSeg.GetProperty("shots").GetProperty("value").GetUInt32());
        generics.Add(GeneralStatTypes.RewardLevel, statsSeg.GetProperty("seasonRewardLevel").GetProperty("value").GetUInt32());
        generics.Add(GeneralStatTypes.ProfileViews, profViews);

        return generics;
    }
    public static Stats? GetStats(string input)
    {
        using (JsonDocument document = JsonDocument.Parse(input))
        {
            if (document.RootElement.TryGetProperty("errors", out _))
            {
                Console.WriteLine("No Stats found");
                return null;
            }

            JsonElement data = document.RootElement.GetProperty("data");

            uint profViews = data.GetProperty("userInfo").GetProperty("pageviews").GetUInt32();
            Date? date = GetDate(data.GetProperty("metadata").GetProperty("lastUpdated").GetProperty("value").GetString()!);
            JsonElement[] segments = data.GetProperty("segments").EnumerateArray().ToArray();

            Dictionary<GeneralStatTypes, uint> generics = new();
            Dictionary<Playlists, Mode> modes = new();

            foreach (JsonElement segment in segments)
            {
                if (segment.GetProperty("type").GetString() == "playlist")
                {
                    Playlists plist;
                    switch (segment.GetProperty("attributes").GetProperty("playlistId").GetUInt16())
                    {
                        case (ushort) Playlists.Casual:
                            plist = Playlists.Casual;
                            break;
                        case (ushort) Playlists.Vs1:
                            plist = Playlists.Vs1;
                            break;
                        case (ushort) Playlists.Vs2:
                            plist = Playlists.Vs2;
                            break;
                        case (ushort) Playlists.Vs3:
                            plist = Playlists.Vs3;
                            break;
                        case (ushort) Playlists.Hoops:
                            plist = Playlists.Hoops;
                            break;
                        case (ushort) Playlists.Rumble:
                            plist = Playlists.Rumble;
                            break;
                        case (ushort) Playlists.Dropshot:
                            plist = Playlists.Dropshot;
                            break;
                        case (ushort) Playlists.Snowday:
                            plist = Playlists.Snowday;
                            break;
                        default:
                            continue;
                    }
                    modes.Add(plist, GetMode(segment.GetProperty("stats"), plist));
                }
                else
                {
                    generics = GetGenerics(segment.GetProperty("stats"), profViews);
                }
            }

            return new Stats(generics, modes, date!);
        }
    }
}
