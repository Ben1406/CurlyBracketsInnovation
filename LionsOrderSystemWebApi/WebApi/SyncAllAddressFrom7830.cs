using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApi;
using WebApi.Data;

namespace webApi;

public static class PublicApiClient
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<HttpResponseMessage?> SyncAllAddressFor7830()
    {
        string endpointUrl = "https://api.danskadresseapi.dk/dawa/adresser?postnr=7830&kommunekode=0661&struktur=flad&per_side=100000";
        const string apiKey = "sk_live_VOBEDc9QgmKXPefAOu5LaZYELBNGw4fNOX_MOzm8kkQ";

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            HttpResponseMessage response = await _httpClient.GetAsync(endpointUrl);
            var statusCode = response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var addresses = JsonSerializer.Deserialize<List<Address>>(jsonResponse);
            if (addresses is not null)
            {
                await StoredInDatabase(addresses);
            }

            return statusCode;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"HTTP eller Autorisationsfejl: {e.Message}");
            return null;
        }
    }

    public static async Task<bool> StoredInDatabase(List<Address> apiAddresses)
    {
        try
        {
            var _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
            var dbAddresses = _dbContext.Addresses.Where(a => a.postnr == "7830").ToList();

            var newAddrFound = 0;

            foreach (var apiAddr in apiAddresses)
            {
                var existsAddress = dbAddresses.SingleOrDefault(a => a.id == apiAddr.id);
                if (existsAddress is null || string.IsNullOrEmpty(existsAddress.id))
                {
                    var newAddress = new Address
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

                    _dbContext.Addresses.Add(newAddress);

                    newAddrFound += 1;
                }
            }

            if (newAddrFound > 0)
            {
                _dbContext.SaveChanges();
                Console.WriteLine($"New addresses found: {newAddrFound}");
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
