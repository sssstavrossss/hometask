using HometaskLib.Models.Dto;

namespace HometaskLib.Application;

public interface ICurrencyService
{
    Task<List<string>> GetAvailableCurrenciesAsync();
    Task<CurrencyConversionResponse> ConvertCurrencyAsync(CurrencyConversionRequest request);
}