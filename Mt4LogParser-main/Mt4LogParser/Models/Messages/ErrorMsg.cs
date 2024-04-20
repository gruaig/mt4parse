using System.Globalization;
using System.Text.RegularExpressions;
using Mt4LogParser.Models.Enums;

namespace Mt4LogParser.Models.Messages;

public class ErrorMsg
{
    public DateTime Timestamp { get; private set; }
    public DbErrorTypeEnum Type { get; private set; }
    public long Value { get; private set; }

    private static readonly Regex Pattern = new (
        @"^(?<Level>[45])\s+(?<Time>\d{2}:\d{2}:\d{2}\.\d{3})\s+(?<Message>.*)$",
        RegexOptions.Compiled);
    
    private static readonly Regex[] ErrorPatterns =
    [
        new Regex(@"MemoryException: (?<Value>\d+) bytes not available", RegexOptions.Compiled),
        new Regex(@"TradeRecord: no enough memory for '[^']+' statements \[(?<Value>\d+)\]", RegexOptions.Compiled),
        new Regex(@"MemPack: reallocate of (?<Value>\d+) bytes failed", RegexOptions.Compiled),
        new Regex(@"TradeBase: no enough memory for copy of index \[(?<Value>\d+)\]", RegexOptions.Compiled),
        new Regex(@"TradeRecord: memory allocation error for orders request \[(?<Value>\d+)\]", RegexOptions.Compiled),
        new Regex(@"^.*error of adding the order #(?<Value>\d+)\s*$", RegexOptions.Compiled)
    ];
    private ErrorMsg(DateTime timestamp, DbErrorTypeEnum type, long value)
    {
        Timestamp = timestamp;
        Type = type;
        Value = value;
    }

    public static bool TryParse(string logLine, DateTime dateTime, out ErrorMsg? errorMsg)
    {
        errorMsg = null;
        var match = Pattern.Match(logLine);
        if (!match.Success) return false;

        DateTime timestamp = dateTime;
        string message = match.Groups["Message"].Value;

        for (int i = 0; i < ErrorPatterns.Length; i++)
        {
            var errorMatch = ErrorPatterns[i].Match(message);
            if (errorMatch.Success)
            {
                var value = long.Parse(errorMatch.Groups["Value"].Value, CultureInfo.InvariantCulture);
                errorMsg = new ErrorMsg(timestamp, (DbErrorTypeEnum)(i + 1), value); // i + 1, because enum error starts at 1
                return true;
            }
        }

        return false;
    }
    
    /*
         5	00:00:01.229		MemoryException: 652125376 bytes not available
         5	00:00:01.229		TradeRecord: no enough memory for 'real' statements [2911018]
         5	09:40:36.532		MemPack: reallocate of 539601973 bytes failed
         4	00:10:45.445		TradeBase: no enough memory for copy of index [28248042]
         5	11:25:49.543		TradeRecord: memory allocation error for orders request [999336]
         5	20:00:12.527	103.18.76.122	'2133007470': error of adding the order #53827380
    */
}