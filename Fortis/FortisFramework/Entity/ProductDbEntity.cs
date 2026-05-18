using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("Product")]
[PrimaryKey("Id")]
public class ProductDbEntity
{
    [Key] [Required] [StringLength(100)]
    public string Id { get; set; } = string.Empty;

    [Required] [StringLength(int.MaxValue)]
    public string Description { get; set; } = string.Empty;

    [Required] [StringLength(int.MaxValue)]
    public bool Blocked { get; set; } = false;
}
