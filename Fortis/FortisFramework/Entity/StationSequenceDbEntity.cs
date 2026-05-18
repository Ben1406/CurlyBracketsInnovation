using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("StationSequence")]
[PrimaryKey("StationName", "SequenceId", "GroupId")]
public class StationSequenceDbEntity
{
    [Key] [Required] [StringLength(100)]
    public string StationName { get; set; } = null!;

    [Required] [StringLength(100)]
    public string SequenceId { get; set; } = null!;

    [Required]
    public int GroupId { get; set; }

    [Required]
    public int SortingId { get; set; }
}
