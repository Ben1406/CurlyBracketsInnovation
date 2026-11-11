using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi;

[Table("produkter")]
[PrimaryKey("id")]
public class Produkter
{
    [Key] [StringLength(100)]
    public required string id { get; set; }

    public required bool aktiv { get; set; }

    public required bool udsolgt { get; set; }

    public required bool ikkefrigivet { get; set; }

    [StringLength(100)]
    public required string gruppenavn { get; set; } = null!;

    [StringLength(100)]
    public required string billedenavn { get; set; } = null!;

    [StringLength(100)]
    public required string beskrivelse { get; set; } = null!;

    [StringLength(100)]
    public string? beskrivelse2 { get; set; } = null!;

    public required int normalprisperstyk { get; set; }

    public required int tilbudsprisperstyk { get; set; }

    public required int tilbudminstyk { get; set; }
}
