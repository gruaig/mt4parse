using System.Globalization;
using System.Text.RegularExpressions;
using Mt4LogParser.Models.Enums;

namespace Mt4LogParser.Models.Messages;

public class LoginMsg
{
    public DateTime Time { get; private set; }
    public bool IsInvestor { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public int Account { get; private set; }
    public string Device { get; private set; } = string.Empty;
    public string Cid { get; private set; } = string.Empty;

    private static readonly Regex Pattern = new (
        @"^2\s+" + // First time login
        @"(?<Time>[^\s]+)\s+" + // Time
        @"(?<IPAddress>[^\s]+)\s+" + // IP address
        @"'(?<Account>\d+)': login\s*" + // Account
        @"\([^,]+,\s*" + // Skipping build number
        @"(?<Device>[^\s]+),\s*" + // Device
        @".*cid:\s*(?<Cid>[^\s,]+).*\)$", // CID
        RegexOptions.Compiled);

    private static readonly Dictionary<string, DbDeviceEnum> Devices = new ()
    {
        {"client", DbDeviceEnum.Client},
        {"android", DbDeviceEnum.Android},
        {"iphone", DbDeviceEnum.IPhone},
    };

    #region methods
    public static bool TryParse(string logLine, DateTime dateTime, out LoginMsg? msg)
    {
        msg = null;

        var what = Pattern.Match(logLine);
        if (!what.Success) return false;

        if (!DateTime.TryParseExact(what.Groups["Time"].Value, "HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time)) return false;

        if (!int.TryParse(what.Groups["Account"].Value, out int account))
        {
            return false;
        }
        
        var ipAddress = what.Groups["IPAddress"].Value;
        var device = what.Groups["Device"].Value;
        var cid = what.Groups["Cid"].Value;
        var isInvestor = logLine.Contains("investor)");

        msg = new LoginMsg
        {
            Time = dateTime,
            IpAddress = ipAddress,
            Account = account,
            Device = device,
            Cid = cid,
            IsInvestor = isInvestor
        };

        return true;
    }
    public DbDeviceEnum GetDbDevice()
    {
        string device = Device;
        Devices.TryGetValue(device, out DbDeviceEnum result);
        return result;
    }
    #endregion
}