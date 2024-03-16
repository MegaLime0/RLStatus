using System.Text.Json;

namespace RLStatus;

public class Mode
{
    public uint MMR { get; private set; }
    public uint Division { get; private set; }
    public Ranks Rank { get; private set; }
    public Playlists Playlist { get; set; }

    public Mode(uint _mmr, uint _div, Ranks _rank, Playlists _plst)
    {
        MMR = _mmr;
        Division = _div;
        Rank = _rank;
        Playlist = _plst;
    }
}
