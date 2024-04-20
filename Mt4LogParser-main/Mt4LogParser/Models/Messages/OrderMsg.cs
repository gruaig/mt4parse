using System.Globalization;
using System.Text.RegularExpressions;
using Mt4LogParser.Models.Entities;
using Mt4LogParser.Models.Enums;

namespace Mt4LogParser.Models.Messages;

public class OrderMsg
{
    public DateTime Time { get; private set; }
    public int Account { get; private set; }
    public string Symbol { get; private set; } = string.Empty;
    public DbOrderOperationEnum Operation { get; private set; }
    public DbEntryOrderEnum Entry { get; private set; }
    public decimal Volume { get; private set; }
    public decimal Bid { get; private set; } 
    public decimal Ask { get; private set; } 
    
    private static readonly Regex Pattern = new (
        @"^1\s+(?<Time>\d{2}:\d{2}:\d{2}\.\d{3})\s+" +
        @"(?<IPAddress>\d{1,3}(?:\.\d{1,3}){3})\s+" +
        @"'(?<Account>\d+)':\s+" +
        @"(market\s+(?<OperationType>buy|sell)\s+(?<Volume>\d+\.\d{2})\s+(?<Symbol>\S+)|" +
        @"close\s+market\s+order\s+#\d+\s+\((?<OperationType>buy|sell)\s+\d+\.\d{2}\s+(?<Symbol>\S+)\s+at\s+\d+\.\d{5}\))\s+" +
        @"sl:\s+\d+\.\d+\s+tp:\s+\d+\.\d+\s+" +
        @"\((?<Bid>\d+\.\d{5})\s*/\s*(?<Ask>\d+\.\d{5})\)$",
        RegexOptions.Compiled);

    public static bool TryParse(string logLine, DateTime dateTime, out OrderMsg? order)
    {
        order = null;
        var match = Pattern.Match(logLine);
        if (!match.Success)
        {
            return false;
        }

        if (!int.TryParse(match.Groups["Account"].Value, out int account))
        {
            return false;
        }

        if (!decimal.TryParse(match.Groups["Volume"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal volume))
        {
            return false;
        }

        if (!decimal.TryParse(match.Groups["Bid"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal bid))
        {
            return false;
        }

        if (!decimal.TryParse(match.Groups["Ask"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal ask))
        {
            return false;
        }

        var operation = match.Groups["OperationType"].Value;
        var operationType = operation.Equals("buy") ? DbOrderOperationEnum.Buy : DbOrderOperationEnum.Sell;
        var entryType = logLine.Contains("market") ? DbEntryOrderEnum.Open : DbEntryOrderEnum.Close;

        order = new OrderMsg
        {
            Time = dateTime,
            Account = account,
            Symbol = match.Groups["Symbol"].Value,
            Operation = operationType,
            Entry = entryType,
            Volume = volume,
            Bid = bid,
            Ask = ask
        };

        return true;
    }
}