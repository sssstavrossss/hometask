using HometaskLib.Models.Fixer;

namespace HometaskLib.Providers.Fixer;

public interface IFixerProvider
{
    Task<FixerResponse> GetRatesAsync(DateTime? date = null, string? baseCurrency = null, string? symbols = null);
    Task<FixerSymbolsResponse> GetSymbolsAsync();
}