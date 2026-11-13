using Microsoft.EntityFrameworkCore;

namespace WebApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Produkter> Produkter { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = "server=mysql38.unoeuro.com;port=3306;user=lcvinderup_dk;password=3hepzt94HcRybAD6gnkm;database=lcvinderup_dk_db;allowpublicKeyretrieval=true";
        optionsBuilder.UseMySQL(connectionString);
    }
}
