using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mt4LogParser.Models.Entities;

[Table("Monitoring")]
public class DbMonitor : BaseEntity
{
    public long? MetaId { get; set; }
    public virtual DbMeta Meta { get; set; }

    public DateTime Timestamp { get; set; }
    public int Connections { get; set; }
    public int FreeMemory { get; set; }
    public int Cpu { get; set; }
    public int Net { get; set; }
    public int Sockets { get; set; }
    public int Threads { get; set; }
    public int Handles { get; set; }
    public int MaxMemoryBlock { get; set; }
    public int ProcessCpu { get; set; }
    public int NetIn { get; set; }
    public int NetOut { get; set; }
}