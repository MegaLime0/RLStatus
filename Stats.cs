namespace RLStatus;

public class Stats
{
    public string Username { get; private set; }

    public Mode? Casual { get; private set; }
    public Mode? Vs1 { get; private set; }
    public Mode? Vs2 { get; private set; }
    public Mode? Vs3 { get; private set; }
    public Mode? Hoops { get; private set; }
    public Mode? Rumble { get; private set; }
    public Mode? Dropshot { get; private set; }
    public Mode? Snowday { get; private set; }

    public uint Wins { get; private set; }
    public uint Goals { get; private set; }
    public uint Saves { get; private set; }
    public uint Assists { get; private set; }
    public uint MVPs { get; private set; }
    public uint Shots { get; private set; }
    public Ranks RewardLevel { get; private set; }
    public uint ProfileViews { get; private set; }

    public bool Success { get; private set; }
    public DateTime Date { get; private set; }

    public Stats(Dictionary<GeneralStatTypes, uint> generics,
            Dictionary<Playlists, Mode> modes,
            DateTime date,
            string username)
    {
        Wins = generics.GetValueOrDefault(GeneralStatTypes.Wins, (uint)0);
        Goals = generics.GetValueOrDefault(GeneralStatTypes.Goals, (uint)0);
        Saves = generics.GetValueOrDefault(GeneralStatTypes.Saves, (uint)0);
        Assists = generics.GetValueOrDefault(GeneralStatTypes.Assists, (uint)0);
        MVPs = generics.GetValueOrDefault(GeneralStatTypes.MVPs, (uint)0);
        Shots = generics.GetValueOrDefault(GeneralStatTypes.Shots, (uint)0);
        RewardLevel = (Ranks)Math.Clamp(generics.GetValueOrDefault(GeneralStatTypes.RewardLevel, (uint)0) * 3 - 1, 0, 22);
        ProfileViews = generics.GetValueOrDefault(GeneralStatTypes.ProfileViews, (uint)0);

        Casual = modes.GetValueOrDefault(Playlists.Casual);
        Vs1 = modes.GetValueOrDefault(Playlists.Vs1);
        Vs2 = modes.GetValueOrDefault(Playlists.Vs2);
        Vs3 = modes.GetValueOrDefault(Playlists.Vs3);
        Hoops = modes.GetValueOrDefault(Playlists.Hoops);
        Rumble = modes.GetValueOrDefault(Playlists.Rumble);
        Dropshot = modes.GetValueOrDefault(Playlists.Dropshot);
        Snowday = modes.GetValueOrDefault(Playlists.Snowday);

        Date = date;
        Username = username;
    }
}
