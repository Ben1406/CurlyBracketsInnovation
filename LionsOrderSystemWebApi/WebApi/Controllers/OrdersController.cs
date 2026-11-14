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
        try
        {
            var ordrer = _dbContext.Ordrer.OrderBy(o => o.ordreid).ToList();
            return await Task.FromResult(Ok(ordrer));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under GetAllOrders: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under GetAllOrders");
        }
    }

    [HttpGet("GetOrderById/{ordreid}")]
    public async Task<ActionResult<Ordrer?>> GetOrderById(int ordreid)
    {
        try
        {
            var ordre = _dbContext.Ordrer.FirstOrDefault(o => o.ordreid == ordreid);

            if (ordre is null) return NotFound("Ordre med id blev ikke fundet.");
            return await Task.FromResult(Ok(ordre));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under GetOrderById: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under GetOrderById");
        }
    }

    [HttpGet("GetLastOrderId")]
    public async Task<ActionResult<Ordrer>> GetLastOrderId()
    {
        try
        {
            var sidsteordre = _dbContext.Ordrer.OrderByDescending(o => o.ordreid).FirstOrDefault();
            if (sidsteordre is null) return new Ordrer()
            {
                ordreid = 0,
                navn = "",
                adresse = "",
                telefon = "",
                mail = "",
                leveringsmetode = "",
                totalpris = 0,
                betalingsmetode = "",
                note = ""
            };
            return await Task.FromResult(Ok(sidsteordre));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under GetLastOrderId: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under GetLastOrderId");
        }
    }

    [HttpGet("GetAllOrderLines/{ordreid}")]
    public async Task<ActionResult<List<OrdreLinjer>>> GetAllOrderLines(int ordreid)
    {
        try
        {
            var ordreLinjer = _dbContext.OrdreLinjer.Where(o => o.ordreid == ordreid).OrderBy(o => o.ordrelinjeid).ToList();
            if (ordreLinjer is null) return NotFound("Ordrelinjer med ordreid blev ikke fundet.");
            return await Task.FromResult(Ok(ordreLinjer));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under GetAllOrderLines: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under GetAllOrderLines");
        }
    }

    [HttpPost("SetNewOrder")]
    public async Task<ActionResult<bool>> SetNewOrder(Ordrer ordrer)
    {
        try
        {
            var nyOrdrer = new Ordrer
            {
                ordreid = ordrer.ordreid,
                bestillingsdato = DateTime.Now.Date,
                navn = ordrer.navn,
                adresse = ordrer.adresse,
                telefon = ordrer.telefon,
                mail = ordrer.mail,
                leveringsmetode = ordrer.leveringsmetode,
                totalpris = ordrer.totalpris,
                betalingsmetode = ordrer.betalingsmetode,
                note = ordrer.note
            };

            _dbContext.Ordrer.Add(nyOrdrer);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(Ok(true));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under SetNewOrder: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under SetNewOrder");
        }
    }

    [HttpPost("SetNewOrderLine")]
    public async Task<ActionResult<bool>> SetNewOrderLine(OrdreLinjer ordreLinjer)
    {
        try
        {
            var nyOrdreLinje = new OrdreLinjer
            {
                ordreid = ordreLinjer.ordreid,
                ordrelinjeid = ordreLinjer.ordrelinjeid,
                produktid = ordreLinjer.produktid,
                produktbeskrivelse = ordreLinjer.produktbeskrivelse,
                produktbeskrivelse2 = ordreLinjer.produktbeskrivelse2,
                antal = ordreLinjer.antal,
                normalprisperstyk = ordreLinjer.normalprisperstyk,
                tilbudsprisperstyk = ordreLinjer.tilbudsprisperstyk,
                tilbudminstyk = ordreLinjer.tilbudminstyk,
                totalpris = ordreLinjer.totalpris
            };

            _dbContext.OrdreLinjer.Add(nyOrdreLinje);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(Ok(true));
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under SetNewOrderLine: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            return NotFound("Fejlede under SetNewOrderLine");
        }
    }
}
