namespace RLStatus;

public sealed class Date
{
    public ushort Year { get; private set; }
    public ushort Month { get; private set; }
    public ushort Day { get; private set; }

    public ushort Hour { get; private set; }
    public ushort Minute { get; private set; }

    public string Raw { get; private set; }

    public Date(string raw)
    {
        ParseRaw(raw);
        Raw = raw.Replace("T", " ");
    }

    public void ParseRaw(string raw)
    {
        Year = Convert.ToUInt16(raw.Substring(0, 4));
        Month = Convert.ToUInt16(raw.Substring(5, 2));
        Day = Convert.ToUInt16(raw.Substring(8, 2));
        Hour = Convert.ToUInt16(raw.Substring(11, 2));
        Minute = Convert.ToUInt16(raw.Substring(14, 2));
    }
}
