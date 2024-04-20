using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mt4LogParser.Models.Enums;

namespace Mt4LogParser.Models.Entities;

[Table("Orders")]
public class DbOrder : BaseEntity
{
    public long? MetaId { get; set; }
    public virtual DbMeta Meta { get; set; }
    public DateTime Time { get; set; }
    public int Account { get; set; }
    [MaxLength(20)] public string Symbol { get; set; } = string.Empty;
    public DbOrderOperationEnum Operation { get; set; }
    public DbEntryOrderEnum Entry { get; set; }
    public decimal Volume { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
}