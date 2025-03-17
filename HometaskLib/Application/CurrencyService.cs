using HometaskLib.Models.Dto;
using HometaskLib.Providers.Fixer;
using HometaskLib.UnitOfWork;

namespace HometaskLib.Application;

public class CurrencyService: ICurrencyService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IFixerProvider _fixerProvider;
        
    public CurrencyService(IUnitOfWorkFactory unitOfWorkFactory, IFixerProvider fixerProvider)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _fixerProvider = fixerProvider;
    }

    public async Task<List<string>> GetAvailableCurrenciesAsync()
    {
        try
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                await uow.BeginTransactionAsync();
                var currencies = await uow.CurrencyRepository.GetExistingCurrenciesAsync();
                await uow.CommitAsync();
                return currencies.Keys.OrderBy(c => c).ToList();
            }
        }
        catch (Exception ex)
        {
            return [];
        }
    }

    public async Task<CurrencyConversionResponse> ConvertCurrencyAsync(CurrencyConversionRequest request)
    {
        Dictionary<string, int> currencies = new Dictionary<string, int>();
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            await uow.BeginTransactionAsync();
            currencies = await uow.CurrencyRepository.GetExistingCurrenciesAsync();
            await uow.CommitAsync();
        }

        if (!currencies.ContainsKey(request.From) || !currencies.ContainsKey(request.To))
        {
            return null;
        }
        
        int baseCurrencyId = currencies[request.From];
        int targetCurrencyId = currencies[request.To];

        decimal? rate;
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            await uow.BeginTransactionAsync();
            // Get the latest conversion rate (or for the provided date)
            rate = await uow.ExchangeRateRepository.GetRateAsync(baseCurrencyId: baseCurrencyId, targetCurrencyId: targetCurrencyId, date: request.Date);
            await uow.CommitAsync();
        }
        
        if (!rate.HasValue)
        {
            var fixerResponse = await _fixerProvider.GetRatesAsync(baseCurrency: request.From, date: request.Date, symbols: request.To);
            if (fixerResponse.Rates.TryGetValue(request.To, out decimal fixerRate))
            {
                rate = fixerRate;
            }
            else
            {
                return null;
            }
        }

        decimal convertedAmount = request.Amount * rate.Value;
        
        return new CurrencyConversionResponse
        {
            From = request.From,
            To = request.To,
            OriginalAmount = request.Amount,
            ConvertedAmount = convertedAmount
        };
    }
}