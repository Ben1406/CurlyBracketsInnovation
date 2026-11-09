using WebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public AddressesController()
    {
        _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<Address>>> GetAllAddresses()
    {
        var roadNames = _dbContext.Addresses.Where(a => a.x_open == true).OrderBy(p => p.betegnelse).ToList();
        return await Task.FromResult(Ok(roadNames));
    }
}
