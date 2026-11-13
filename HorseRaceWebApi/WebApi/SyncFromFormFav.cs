using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApi;
using WebApi.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace webApi;

public static class PublicApiClient
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<HttpResponseMessage?> SyncFromFormFav()
    {
        try
        {
            string date = "2026-11-13";
            string track = "Skive";
            int race = 1;
            string raceCode = "harness"; // galop = gallops, trav = harness, greyhound = greyhounds
            string country = "dk";

            // DAGENS LØB PÅ ALLE BANER:
            // string endpointUrl = $"/v1/form/meetings?date={date}&race_code={raceCode}";
            // DAGENS HESTE I ÉT LØB PÅ ÉN BANE:
            // string endpointUrl = $"v1/form?date={date}&track={Uri.EscapeDataString(track)}&race={race}&race_code={raceCode}&country={country}";
            // ALLE BANER:
            // string endpointUrl = $"/v1/form/venues?raceType={raceCode}";
            string endpointUrl = $"v1/form?date={date}&track={Uri.EscapeDataString(track)}&race={race}&race_code={raceCode}&country={country}";
            const string apiKey = "fk_aefab52aeb0b430d49c6b95bc76e58236233f6a450ab408c70fee92eff11c503";

            _httpClient.BaseAddress = new Uri("https://api.formfav.com/");
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

            HttpResponseMessage response = await _httpClient.GetAsync(endpointUrl);
            var statusCode = response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            //var adresser = JsonSerializer.Deserialize<List<Adresser>>(jsonResponse);
            //if (adresser is not null)
            //{
            //    await StoredInDatabase(adresser);
            //}

            return statusCode;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"HTTP eller Autorisationsfejl: {e.Message}");
            return null;
        }
    }

    //public static async Task<bool> StoredInDatabase(List<Adresser> apiAdresser)
    //{
    //    try
    //    {
    //        var _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
    //        var dbAdresser = _dbContext.Adresser.Where(a => a.postnr == "7830").ToList();

    //        var newAddrFound = 0;

    //        foreach (var apiAddr in apiAdresser)
    //        {
    //            var existsAddress = dbAdresser.SingleOrDefault(a => a.id == apiAddr.id);
    //            if (existsAddress is null || string.IsNullOrEmpty(existsAddress.id))
    //            {
    //                var newAddress = new Adresser
    //                {
    //                    id = apiAddr.id,
    //                    status = apiAddr.status,
    //                    darstatus = apiAddr.darstatus,
    //                    vejkode = apiAddr.vejkode,
    //                    vejnavn = apiAddr.vejnavn,
    //                    husnr = apiAddr.husnr,
    //                    etage = apiAddr.etage,
    //                    dør = apiAddr.dør,
    //                    supplerendebynavn = apiAddr.supplerendebynavn,
    //                    postnr = apiAddr.postnr,
    //                    postnrnavn = apiAddr.postnrnavn,
    //                    kommunekode = apiAddr.kommunekode,
    //                    kommunenavn = apiAddr.kommunenavn,
    //                    matrikelnr = apiAddr.matrikelnr,
    //                    regionskode = apiAddr.regionskode,
    //                    regionsnavn = apiAddr.regionsnavn,
    //                    sognekode = apiAddr.sognekode,
    //                    sognenavn = apiAddr.sognenavn,
    //                    betegnelse = apiAddr.betegnelse
    //                };

    //                _dbContext.Adresser.Add(newAddress);

    //                newAddrFound += 1;
    //            }
    //        }

    //        if (newAddrFound > 0)
    //        {
    //            _dbContext.SaveChanges();
    //            Console.WriteLine($"Nye adresser fundet: {newAddrFound}");
    //        }

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        return false;
    //    }
    //}
}
