using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("Station")]
[PrimaryKey("Name")]
public class StationDbEntity
{
    [Key] [Required] [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required] [StringLength(int.MaxValue)]
    public string Description { get; set; } = null!;

    [Required] [StringLength(100)]
    public string ColorTheme { get; set; } = null!;

    [Required] [StringLength(100)]
    public string ScalingMode { get; set; } = null!;

    [Required]
    public double ScalingFactor { get; set; } = 1;

    [Required] [StringLength(100)]
    public string WindowStyle { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Language { get; set; } = null!;
}
