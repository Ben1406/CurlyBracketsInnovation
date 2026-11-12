using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public OrdersController()
    {
        _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
    }

    [HttpGet("GetAllOrders")]
    public async Task<ActionResult<List<Ordrer>>> GetAllOrders()
    {
        var ordrer = _dbContext.Ordrer.OrderBy(o => o.ordreid).ToList();
        return await Task.FromResult(Ok(ordrer));
    }

    [HttpGet("GetOrderById/{ordreid}")]
    public async Task<ActionResult<Ordrer?>> GetOrderById(int ordreid)
    {
        var ordre = _dbContext.Ordrer.FirstOrDefault(o => o.ordreid == ordreid);

        if (ordre is null) return NotFound("Ordre med id blev ikke fundet.");
        return await Task.FromResult(Ok(ordre));
    }

    [HttpGet("GetLastOrderId")]
    public async Task<ActionResult<Ordrer>> GetLastOrderId()
    {
        var sidsteordre = _dbContext.Ordrer.OrderByDescending(o => o.ordreid).FirstOrDefault();
        if (sidsteordre is null) return new Ordrer()
        {
            ordreid = 0,
            bestillingsdato = DateTime.Now,
            navn = "",
            adresse = "",
            telefon = "",
            mail = "",
            stilles = "",
            totalpris = 0
        };
        return await Task.FromResult(Ok(sidsteordre));
    }

    [HttpGet("GetAllOrderLines/{ordreid}")]
    public async Task<ActionResult<List<OrdreLinjer>>> GetAllOrderLines(int ordreid)
    {
        var ordreLinjer = _dbContext.OrdreLinjer.Where(o => o.ordreid == ordreid).OrderBy(o => o.ordrelinjeid).ToList();
        if (ordreLinjer is null) return NotFound("Ordrelinjer med ordreid blev ikke fundet.");
        return await Task.FromResult(Ok(ordreLinjer));
    }

    [HttpPost("SetNewOrder")]
    public async Task<ActionResult<bool>> SetNewOrder(int ordreid, string navn, string adresse, string telefon, string? mail, string? stilles, int totalpris)
    {
        try
        {
            var nyOrdrer = new Ordrer
            {
                ordreid = ordreid,
                bestillingsdato = DateTime.Now.Date,
                navn = navn,
                adresse = adresse,
                telefon = telefon,
                mail = mail,
                stilles = stilles,
                totalpris = totalpris
            };

            _dbContext.Ordrer.Add(nyOrdrer);
            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(Ok(true));
        }
        catch
        {
            return BadRequest("Kunne ikke gemme ordre i databasen.");
        }
    }

    [HttpPost("SetNewOrderLine")]
    public async Task<ActionResult<bool>> SetNewOrderLine(int ordreid, int orderlinjeid, string produktid, string produktbeskrivelse, string? produktbeskrivelse2, int antal, int normalprisperstyk, int? tilbudsprisperstyk, int? tilbudminstyk)
    {
        try
        {
            var nyOrdreLinje = new OrdreLinjer
            {
                ordreid = ordreid,
                ordrelinjeid = orderlinjeid,
                produktid = produktid,
                produktbeskrivelse = produktbeskrivelse,
                produktbeskrivelse2 = produktbeskrivelse2,
                antal = antal,
                normalprisperstyk = normalprisperstyk,
                tilbudsprisperstyk = tilbudsprisperstyk,
                tilbudminstyk = tilbudminstyk
            };

            _dbContext.OrdreLinjer.Add(nyOrdreLinje);
            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(Ok(true));
        }
        catch
        {
            return BadRequest("Kunne ikke gemme ordrelinje i databasen.");
        }
    }

}
