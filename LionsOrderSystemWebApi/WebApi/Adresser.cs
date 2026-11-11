using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApi;

[Table("adresser")]
[PrimaryKey("id")]
public class Adresser
{
    [Key]
    public string id { get; set; } = null!;

    public int? status { get; set; }
    public int? darstatus { get; set; }
    public string? vejkode { get; set; }
    public string? vejnavn { get; set; }
    public string? husnr { get; set; }
    public string? etage { get; set; }
    public string? dør { get; set; }
    public string? supplerendebynavn { get; set; }
    public string? postnr { get; set; }
    public string? postnrnavn { get; set; }
    public string? kommunekode { get; set; }
    public string? kommunenavn { get; set; }
    public string? matrikelnr { get; set; }
    public string? regionskode { get; set; }
    public string? regionsnavn { get; set; }
    public string? sognekode { get; set; }
    public string? sognenavn { get; set; }
    public string? betegnelse { get; set; }

    public bool? x_aktiv { get; set; }
    public int? x_område { get; set; }
    public string? x_rute { get; set; }
    public int? x_lionsnr { get; set; }
    public int? x_hjælpernr { get; set; }
}
