using System.Globalization;
using System.Text.RegularExpressions;

namespace Mt4LogParser.Models.Messages;

public class MonitorMsg
{
    public DateTime Timestamp { get; private set; }
    public int Connections { get; private set; }
    public int FreeMemory { get; private set; }
    public int Cpu { get; private set; }
    public int Net { get; private set; }
    public int Sockets { get; private set; }
    public int Threads { get; private set; }
    public int Handles { get; private set; }
    public int MaxMemoryBlock { get; private set; }
    public int ProcessCpu { get; private set; }
    public int NetIn { get; private set; }
    public int NetOut { get; private set; }

    //0	00:18:01.201	Monitor	connections: 1270  free memory: 1073368 kb  cpu: 5%  net: 0 Kbyte/s sockets: 1486 threads: 67 handles: 1829
    private static readonly Regex Pattern = new (
        @"^0\s+(?<Time>[^\s]+)\s+Monitor\s+" +
        @"connections:\s+(?<Connections>\d+)\s+" +
        @"free memory:\s+(?<FreeMemory>\d+)\s+kb\s+" +
        @"cpu:\s+(?<Cpu>\d+)%\s+" +
        @"net:\s+(?<Net>\d+)\s+Kbyte/s\s+" +
        @"sockets:\s+(?<Sockets>\d+)\s+" +
        @"threads:\s+(?<Threads>\d+)\s+" +
        @"handles:\s+(?<Handles>\d+).*$", 
        RegexOptions.Compiled
    );

    //0	00:10:01.222	Monitor max.memory block: 551224 kb process cpu: 0%  net in: 0 Kbyte/s net out: 0 Kbyte/s
    private static readonly Regex PatternAdditional = new (
        @"^0\s+(?<Time>[^\s]+)\s+Monitor\s+" +
        @"max\. memory block:\s+(?<MaxMemoryBlock>\d+)\s+kb\s+" +
        @"process cpu:\s+(?<ProcessCpu>\d+)%\s+" +
        @"net in:\s+(?<NetIn>\d+)\s+Kbyte/s\s+" +
        @"net out:\s+(?<NetOut>\d+)\s+Kbyte/s.*$", 
        RegexOptions.Compiled
    );

    #region methods

    public static bool TryParse(string logLine, DateTime dateTime, out MonitorMsg? msg)
    {
        msg = null;
        var match = Pattern.Match(logLine);
        if (!match.Success) return false;

        if (!DateTime.TryParseExact(match.Groups["Time"].Value, "HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
        {
            return false;
        }

        msg = new MonitorMsg
        {
            Timestamp = dateTime,
            Connections = int.Parse(match.Groups["Connections"].Value, CultureInfo.InvariantCulture),
            FreeMemory = int.Parse(match.Groups["FreeMemory"].Value, CultureInfo.InvariantCulture),
            Cpu = int.Parse(match.Groups["Cpu"].Value, CultureInfo.InvariantCulture),
            Net = int.Parse(match.Groups["Net"].Value, CultureInfo.InvariantCulture),
            Sockets = int.Parse(match.Groups["Sockets"].Value, CultureInfo.InvariantCulture),
            Threads = int.Parse(match.Groups["Threads"].Value, CultureInfo.InvariantCulture),
            Handles = int.Parse(match.Groups["Handles"].Value, CultureInfo.InvariantCulture),
        };

        return true;
    }

    public bool TryParseAdditional(string logLine)
    {
        var match = PatternAdditional.Match(logLine);
        if (!match.Success)
        {
            return false;
        }

        MaxMemoryBlock = int.Parse(match.Groups["MaxMemoryBlock"].Value, CultureInfo.InvariantCulture);
        ProcessCpu = int.Parse(match.Groups["ProcessCpu"].Value, CultureInfo.InvariantCulture);
        NetIn = int.Parse(match.Groups["NetIn"].Value, CultureInfo.InvariantCulture);
        NetOut = int.Parse(match.Groups["NetOut"].Value, CultureInfo.InvariantCulture);

        return true;
    }

    #endregion
}