using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi;

[Table("Product")]
[PrimaryKey("Id")]
public class Product
{
    [Key] [StringLength(100)]
    public required string Id { get; set; }

    public required bool Active { get; set; }

    public required bool SoldOut { get; set; }

    [StringLength(100)]
    public required string GroupName { get; set; } = null!;

    [StringLength(100)]
    public required string ImageName { get; set; } = null!;

    [StringLength(int.MaxValue)]
    public required string Description { get; set; } = null!;

    [StringLength(int.MaxValue)]
    public string? Description2 { get; set; } = null!;

    public required int NormalPricePerPiece { get; set; }

    public required int OfferPricePerPiece { get; set; }

    public required int OfferMinPieces { get; set; }
}
