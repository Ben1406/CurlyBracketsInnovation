using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("Sequence")]
[PrimaryKey("Id")]
public class SequenceDbEntity
{
    [Key] [Required] [StringLength(100)]
    public string Id { get; set; } = null!;

    [Required] [StringLength(int.MaxValue)]
    public string Description { get; set; } = null!;

    [StringLength(int.MaxValue)]
    public string? SubDescription { get; set; } = null!;
}
