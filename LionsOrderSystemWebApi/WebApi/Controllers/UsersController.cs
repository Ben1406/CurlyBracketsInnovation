using WebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public UsersController()
    {
        _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
    }

    [HttpPost("ValidateUser")]
    public async Task<ActionResult<List<Brugere>>> ValidateUser(Brugere bruger)
    {
        try
        {
            var brugere = _dbContext.Brugere.Where(a => a.initialer == bruger.initialer).ToList();
            if (brugere.Count != 1 || brugere[0].adgangskode != bruger.adgangskode)
            {
                return await Task.FromResult(NotFound(null));
            }
            brugere[0].adgangskode = "";
            return await Task.FromResult(Ok(brugere));
        }
        catch (Exception ex)
        {
            Log.Write($"Fejlede under ValidateUser: {ex}");
            return NotFound("Fejlede under ValidateUser");
        }
    }
}
