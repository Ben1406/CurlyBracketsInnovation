using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi;

[Table("ordrer")]
[PrimaryKey("ordreid")]
public class Ordrer
{
    [Key]
    public required int ordreid { get; set; }

    public required DateTime bestillingsdato { get; set; }
    public DateTime? betalingsdato { get; set; }
    public required string navn { get; set; } = null!;
    public required string adresse { get; set; } = null!;
    public required string telefon { get; set; } = null!;
    public string? mail { get; set; } = null!;
    public string? stilles { get; set; } = null!;
    public required int totalpris { get; set; }
}

[Table("ordrelinjer")]
[PrimaryKey("ordreid", "ordrelinjeid")]
public class OrdreLinjer
{
    [Key]
    public required int ordreid { get; set; }
    [Key]
    public required int ordrelinjeid { get; set; }

    public required string produktid { get; set; } = null!;
    public required string produktbeskrivelse { get; set; } = null!;
    public string? produktbeskrivelse2 { get; set; } = null!;
    public required int antal { get; set; }
    public required int normalprisperstyk { get; set; }
    public int? tilbudsprisperstyk { get; set; }
    public int? tilbudminstyk { get; set; }
}
