using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApi;

[Table("Address")]
[PrimaryKey("id")]
public class Address
{
    [Key]
    public required string id { get; set; } = null!;

    public int status { get; set; }
    public int darstatus { get; set; }
    public string vejkode { get; set; } = null!;
    public string vejnavn { get; set; } = null!;
    public string husnr { get; set; } = null!;
    public string etage { get; set; } = null!;
    public string dør { get; set; } = null!;
    public string supplerendebynavn { get; set; } = null!;
    public string postnr { get; set; } = null!;
    public string postnrnavn { get; set; } = null!;
    public string kommunekode { get; set; } = null!;
    public string kommunenavn { get; set; } = null!;
    public string matrikelnr { get; set; } = null!;
    public string regionskode { get; set; } = null!;
    public string regionsnavn { get; set; } = null!;
    public string sognekode { get; set; } = null!;
    public string sognenavn { get; set; } = null!;
    public string betegnelse { get; set; } = null!;
}
