namespace RLStatus;

public class Stats
{
    public Mode Vs1 { get; private set; }
    public Mode Vs2 { get; private set; }
    public Mode Vs3 { get; private set; }

    public uint Wins { get; private set; }
    public uint Goals { get; private set; }
    public uint Saves { get; private set; }
    public uint Assists { get; private set; }
    public uint MVPs { get; private set; }
    public uint Shots { get; private set; }

    public Stats()
    {

    }
}
