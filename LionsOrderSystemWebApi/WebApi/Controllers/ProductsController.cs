using WebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ProductsController()
    {
        _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
    }

    [HttpGet("GetAllProducts/{gruppenavn}")]
    public async Task<ActionResult<List<Produkter>>> GetAllProducts(string gruppenavn)
    {
        try
        {
            var produkter = _dbContext.Produkter.Where(p => p.gruppenavn == gruppenavn).OrderBy(p => p.id).ToList();
            return await Task.FromResult(Ok(produkter));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under GetAllProducts: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under GetAllProducts");
        }
    }

    [HttpGet("GetProductById/{id}")]
    public async Task<ActionResult<Produkter?>> GetProductById(string id)
    {
        try
        {
            var produkt = _dbContext.Produkter.FirstOrDefault(p => p.id == id);

            if (produkt is null) return NotFound("Produkt med id blev ikke fundet.");
            return await Task.FromResult(Ok(produkt));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under GetProductById: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under GetProductById");
        }
    }
}
