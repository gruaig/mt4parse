using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mt4LogParser.Models.Enums;

namespace Mt4LogParser.Models.Entities;

[Table("Activities")]
public class DbActivity : BaseEntity
{
    public long? MetaId { get; set; }
    public virtual DbMeta Meta { get; set; }
    public int Account { get; set; }

    [StringLength(32)] public string Cid { get; set; } = string.Empty;
    public DateTime FirstLoginTime { get; set; }
    public DateTime LastLoginTime { get; set; }
    public bool IsInvestor { get; set; }

    [StringLength(32)] public string IpAddress { get; set; } = string.Empty;
    public DbDeviceEnum Device { get; set; }

    public int NumberOfLogins { get; set; }
    public int NumberOfOrders { get; set; }
        
}