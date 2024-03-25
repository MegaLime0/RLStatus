using System.Text.Json;

namespace RLStatus;

public class Mode
{
    public int MMR { get; private set; }
    public int Division { get; private set; }
    public Ranks Rank { get; private set; }
    public Playlists Playlist { get; set; }

    public Mode(int _mmr, int _div, Ranks _rank, Playlists _plst)
    {
        MMR = _mmr;
        Division = _div;
        Rank = _rank;
        Playlist = _plst;
    }
}
