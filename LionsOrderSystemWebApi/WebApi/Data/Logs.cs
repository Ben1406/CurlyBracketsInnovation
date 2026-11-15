using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data;

[Table("logs")]
[PrimaryKey("id")]
public class Logs
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    public required DateTime tidspunkt { get; set; }
    public required string besked { get; set; } = null!;
}

public static class Log
{
    private static AppDbContext dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());

    public static void Write(string besked)
    {
        var log = new Logs()
        {
            tidspunkt = DateTime.Now,
            besked = besked
        };
        dbContext.Log.Add(log);
        dbContext.SaveChanges();
    }
}
