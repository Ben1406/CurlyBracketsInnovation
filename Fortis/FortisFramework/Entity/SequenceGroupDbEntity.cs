using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("SequenceGroup")]
[PrimaryKey("Id")]
public class SequenceGroupDbEntity
{
    [Key] [Required]
    public int Id { get; set; }

    [Required] [StringLength(int.MaxValue)]
    public string Description { get; set; } = null!;
}
