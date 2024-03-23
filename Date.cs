namespace RLStatus;

public sealed class Date
{
    public ushort Year { get; private set; }
    public ushort Month { get; private set; }
    public ushort Day { get; private set; }

    public ushort Hour { get; private set; }
    public ushort Minute { get; private set; }

    public Date(ushort year, ushort month, ushort day, ushort hour, ushort minute)
    {
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
    }
}
