using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mt4LogParser.Models.Entities;

[Table("States")]
public class DbState : BaseEntity
{
    public long? MetaId { get; set; }
    public virtual DbMeta Meta { get; set; }
    public string Path { get; set; } = string.Empty;
    public long Position { get; set; }
}