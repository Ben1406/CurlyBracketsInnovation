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

    [HttpGet("GetAll/{gruppenavn}")]
    public async Task<ActionResult<List<Produkter>>> GetAllProducts(string gruppenavn)
    {
        var produkter = _dbContext.Produkter.Where(p => p.gruppenavn == gruppenavn).OrderBy(p => p.id).ToList();
        return await Task.FromResult(Ok(produkter));
    }

    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<Produkter?>> GetProductById(string id)
    {
        var produkter = _dbContext.Produkter.FirstOrDefault(p => p.id == id);

        if (produkter is null) return NotFound("Produktet med id blev ikke fundet.");
        return await Task.FromResult(Ok(produkter));
    }
}
