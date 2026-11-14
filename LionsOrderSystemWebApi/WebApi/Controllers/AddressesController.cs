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

    [HttpGet("GetAllAddresses")]
    public async Task<ActionResult<List<Adresser>>> GetAllAddresses()
    {
        try
        {
            var adresser = _dbContext.Adresser.Where(a => a.x_aktiv == true).OrderBy(p => p.betegnelse).ToList();
            return await Task.FromResult(Ok(adresser));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under GetAllAddresses: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under GetAllAddresses");
        }
    }
}
