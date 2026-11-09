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

    public static async Task<bool> StoredInDatabase(List<Address> addresses)
    {
        try
        {
            var _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
            var existingAddresses = _dbContext.Addresses.Where(a => a.postnr == "7830").ToList();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
