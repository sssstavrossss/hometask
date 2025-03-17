using HometaskLib.Models.Fixer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HometaskLib.Providers.Fixer;

public class FixerProvider: IFixerProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
        
    public FixerProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Fixer:AccessKey"];
    }

    public async Task<FixerResponse> GetRatesAsync(DateTime? date = null, string? baseCurrency = null, string? symbols = null)
    {
        // Determine the endpoint based on whether a date is provided.
        string endpoint = date.HasValue ? date.Value.ToString("yyyy-MM-dd") : "latest";
        // Start building the URL with the required access_key.
        var url = $"http://data.fixer.io/api/{endpoint}?access_key={_apiKey}";
    
        // Append the base currency if provided.
        if (!string.IsNullOrEmpty(baseCurrency))
        {
            url += $"&base={baseCurrency}";
        }
        // Append symbols if provided (comma-separated list).
        if (!string.IsNullOrEmpty(symbols))
        {
            url += $"&symbols={symbols}";
        }
    
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FixerResponse>(json);
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }
    
    public async Task<FixerSymbolsResponse> GetSymbolsAsync()
    {
        string url = $"http://data.fixer.io/api/symbols?access_key={_apiKey}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FixerSymbolsResponse>(json);
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }
}