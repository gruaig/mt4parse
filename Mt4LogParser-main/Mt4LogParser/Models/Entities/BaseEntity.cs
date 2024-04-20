using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mt4LogParser.Models.Interfaces;

namespace Mt4LogParser.Models.Entities;

public class BaseEntity : IEntity<long>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
}