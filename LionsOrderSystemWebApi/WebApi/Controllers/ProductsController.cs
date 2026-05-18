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

    [HttpGet("GetAll/{groupName}")]
    public async Task<ActionResult<List<Product>>> GetAllProducts(string groupName)
    {
        var products = _dbContext.Products.Where(p => p.GroupName == groupName).OrderBy(p => p.Id).ToList();
        return await Task.FromResult(Ok(products));
    }

    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<Product?>> GetProductById(string id)
    {
        var products = _dbContext.Products.FirstOrDefault(p => p.Id == id);

        if (products is null) return NotFound("The product with the given id was not found.");
        return await Task.FromResult(Ok(products));
    }
}
