using System.Globalization;
using System.Text.RegularExpressions;

namespace Mt4LogParser.Models.Messages;

public class TradeMsg
{
    public DateTime Time { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public int Account { get; private set; }
    public int Order { get; private set; }

    private static readonly Regex Pattern = new (
        @"^1\s+" + // Marking the start of a trade message
        @"(?<Time>[^\s]+)\s+" + // Capturing trade time
        @"(?<IPAddress>[^\s]+)\s+" + // Capturing IP address
        @"'(?<Account>\d+)'\s*:\s+" + // Capturing account number
        @"order\s+#(?<Order>\d+)" + // Capturing order number
        @".*", // Ignoring the rest
        RegexOptions.Compiled);

    #region methods
    public static bool TryParse(string logLine, DateTime dateTime, out TradeMsg? msg)
    {
        msg = null;
        var match = Pattern.Match(logLine);
        if (!match.Success)
        {
            return false;
        }

        if (!int.TryParse(match.Groups["Account"].Value, out int account))
        {
            return false;
        }

        if (!int.TryParse(match.Groups["Order"].Value, out int order))
        {
            return false;
        }

        msg = new TradeMsg
        {
            Time = dateTime,
            IpAddress = match.Groups["IPAddress"].Value,
            Account = account,
            Order = order
        };

        return true;
    }
    #endregion

    #region examples
    /*
    1	18:01:43.303	134.17.5.122	'20003': order #50442, buy limit 0.05 EURUSD at 1.21759
    1	18:01:50.500	134.17.5.122	'20003': close market order #50435 (sell 0.05 EURUSD at 1.21721) (1.21763 / 1.21787)
    1	18:01:50.532	134.17.5.122	'20003': close order #50435 (sell 0.05 EURUSD at 1.21721) at 1.21787 completed
    1	18:01:57.776	134.17.5.122	'20003': close order #50440 (sell 0.92 EURUSD at 1.21811) by #50437 (buy 1.93 at 1.21744)
    1	18:01:57.776	134.17.5.122	'20003': order #50443, buy 1.01 EURUSD at 1.21744
    1	18:01:57.776	134.17.5.122	'20003': close order #50440 by #50437 at 1.21744 completed
    */
    #endregion
}