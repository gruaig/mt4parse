using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mt4LogParser.Models.Enums;

namespace Mt4LogParser.Models.Entities;

[Table("Errors")]
public class DbError : BaseEntity
{
    public long? MetaId { get; set; }
    public virtual DbMeta Meta { get; set; }
    public DateTime Timestamp { get; set; }
    public DbErrorTypeEnum Type { get; set; }
    public long Value { get; set; }
}