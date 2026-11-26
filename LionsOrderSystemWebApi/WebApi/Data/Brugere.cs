using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data;

[Table("brugere")]
[PrimaryKey("initialer")]
public class Brugere
{
    [Key]
    public required string initialer { get; set; } = null!;

    public required string navn { get; set; } = null!;
    public required string adgangskode { get; set; } = null!;
}
