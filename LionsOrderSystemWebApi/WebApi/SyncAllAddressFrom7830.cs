using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApi.Data;

namespace webApi;

public static class PublicApiClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static readonly AppDbContext _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());

    public static async Task<HttpResponseMessage?> SyncAllAddressFor7830()
    {
        string endpointUrl = "https://api.danskadresseapi.dk/dawa/adresser?postnr=7830&kommunekode=0661&struktur=flad&per_side=100000";
        const string apiKey = "sk_live_VOBEDc9QgmKXPefAOu5LaZYELBNGw4fNOX_MOzm8kkQ";

        try
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = "Læser adresser fra 'api.danskadresseapi.dk'"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            HttpResponseMessage response = await _httpClient.GetAsync(endpointUrl);
            var statusCode = response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var adresser = JsonSerializer.Deserialize<List<Adresser>>(jsonResponse);
            if (adresser is not null)
            {
                await StoredInDatabase(adresser);
            }

            return statusCode;
        }
        catch (HttpRequestException ex)
        {
            var log = new Log()
            {
                tidspunkt = DateTime.Now,
                besked = $"Fejlede under SyncAllAddressFor7830: {ex}"
            };
            _dbContext.Log.Add(log);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"HTTP eller Autorisationsfejl: {ex.Message}");
            return null;
        }
    }

    public static async Task<bool> StoredInDatabase(List<Adresser> apiAdresser)
    {
        try
        {
            var _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
            var dbAdresser = _dbContext.Adresser.Where(a => a.postnr == "7830").ToList();

            var newAddrFound = 0;

            foreach (var apiAddr in apiAdresser)
            {
                var existsAddress = dbAdresser.SingleOrDefault(a => a.id == apiAddr.id);
                if (existsAddress is null || string.IsNullOrEmpty(existsAddress.id))
                {
                    var newAddress = new Adresser
                    {
                        id = apiAddr.id,
                        status = apiAddr.status,
                        darstatus = apiAddr.darstatus,
                        vejkode = apiAddr.vejkode,
                        vejnavn = apiAddr.vejnavn,
                        husnr = apiAddr.husnr,
                        etage = apiAddr.etage,
                        dør = apiAddr.dør,
                        supplerendebynavn = apiAddr.supplerendebynavn,
                        postnr = apiAddr.postnr,
                        postnrnavn = apiAddr.postnrnavn,
                        kommunekode = apiAddr.kommunekode,
                        kommunenavn = apiAddr.kommunenavn,
                        matrikelnr = apiAddr.matrikelnr,
                        regionskode = apiAddr.regionskode,
                        regionsnavn = apiAddr.regionsnavn,
                        sognekode = apiAddr.sognekode,
                        sognenavn = apiAddr.sognenavn,
                        betegnelse = apiAddr.betegnelse
                    };

                    _dbContext.Adresser.Add(newAddress);

                    newAddrFound += 1;
                }
            }

            if (newAddrFound > 0)
            {
                _dbContext.SaveChanges();

                var log = new Log()
                {
                    tidspunkt = DateTime.Now,
                    besked = $"Fandt '{newAddrFound}' nye adresser"
                };
                _dbContext.Log.Add(log);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"Nye adresser fundet: {newAddrFound}");
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
