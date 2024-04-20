using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mt4LogParser.Models.Entities;

[Table("Metas")]
public class DbMeta : BaseEntity
{
    [Required]
    [StringLength(64)]
    public required string Name { get; set; }
    public List<DbActivity> Activities { get; set; }
    public List<DbState> States { get; set; }
    public List<DbMonitor> Monitors { get; set; }
    public List<DbError> Errors { get; set; }
    public List<DbOrder> Orders { get; set; }
}