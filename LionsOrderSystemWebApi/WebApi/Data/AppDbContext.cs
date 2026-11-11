using Microsoft.EntityFrameworkCore;

namespace WebApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Produkter> Produkter { get; set; }
    public DbSet<Adresser> Adresser { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");
        optionsBuilder.UseMySQL(connectionString);
    }
}
