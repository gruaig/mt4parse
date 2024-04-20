using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mt4LogParser.Models.Interfaces;

public interface IEntity<T>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    T Id { get; set; }
}