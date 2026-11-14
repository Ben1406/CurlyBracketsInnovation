using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data;

[Table("log")]
[PrimaryKey("id")]
public class Log
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    public required DateTime tidspunkt { get; set; }
    public required string besked { get; set; } = null!;
}
