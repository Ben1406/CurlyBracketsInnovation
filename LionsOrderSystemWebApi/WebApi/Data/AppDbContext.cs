using Microsoft.EntityFrameworkCore;

namespace WebApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Logs> Log { get; set; }
    public DbSet<Produkter> Produkter { get; set; }
    public DbSet<Adresser> Adresser { get; set; }
    public DbSet<Ordrer> Ordrer{ get; set; }
    public DbSet<OrdreLinjer> OrdreLinjer { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = Globals.dbConnectionString;
        optionsBuilder.UseMySQL(connectionString);
    }
}
