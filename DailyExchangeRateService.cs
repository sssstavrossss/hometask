using HometaskLib.Models.Fixer;
using HometaskLib.Providers.Fixer;
using HometaskLib.UnitOfWork;

namespace Hometask;

public class DailyExchangeRateService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IFixerProvider _fixerProvider;
        private readonly ILogger<DailyExchangeRateService> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DailyExchangeRateService(IFixerProvider fixerProvider,
                                        ILogger<DailyExchangeRateService> logger,
                                        IUnitOfWorkFactory unitOfWorkFactory)
        {
            _fixerProvider = fixerProvider;
            _logger = logger;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Schedule the service to run at the next midnight (UTC) and then every 24 hours.
            var now = DateTime.UtcNow;
            var nextRunTime = now.Date.AddDays(-1);
            var initialDelay = nextRunTime - now;
            if (initialDelay < TimeSpan.Zero)
                initialDelay = TimeSpan.Zero;
            _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("DailyExchangeRateService started at: {Time}", DateTime.Now);
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                try
                {
                    await uow.BeginTransactionAsync();
                    var today = DateTime.UtcNow.Date;

                    // Check if today's exchange rates already exist.
                    if (await uow.DateDimensionRepository.DateExistsAsync(today))
                    {
                        _logger.LogInformation("Exchange rates for {Date} already exist. Skipping update.", today.ToShortDateString());
                        await uow.CommitAsync();
                        return;
                    }

                    // Insert today's date and get DateID.
                    int dateId = await uow.DateDimensionRepository.InsertDateDimensionAsync(today);

                    // Retrieve supported symbols from Fixer.
                    var symbolsResponse = await _fixerProvider.GetSymbolsAsync();
                    if (symbolsResponse == null || !symbolsResponse.Success)
                    {
                        _logger.LogError("Failed to retrieve symbols from Fixer.io.");
                        symbolsResponse = new FixerSymbolsResponse() { Symbols = new Dictionary<string, string>(), Success = false };
                    }

                    // Get existing currencies.
                    var existingCurrencies = await uow.CurrencyRepository.GetExistingCurrenciesAsync();

                    // Insert any new currencies.
                    foreach (var kvp in symbolsResponse.Symbols)
                    {
                        var currencyCode = kvp.Key;
                        var currencyName = kvp.Value;
                        if (!existingCurrencies.ContainsKey(currencyCode))
                        {
                            int newCurrencyId = await uow.CurrencyRepository.InsertCurrencyAsync(currencyCode, currencyName);
                            if (newCurrencyId != 0) existingCurrencies[currencyCode] = newCurrencyId;
                        }
                    }

                    // Process rates for the base currency "EUR" (per free Fixer plan).
                    if (existingCurrencies.TryGetValue("EUR", out int baseCurrencyId))
                    {
                        var fixerResponse = await _fixerProvider.GetRatesAsync(baseCurrency: "EUR", date: today);
                        if (fixerResponse == null || !fixerResponse.Success)
                        {
                            _logger.LogError("Failed to retrieve rates for base currency EUR");
                        }
                        else
                        {
                            foreach (var rateKvp in fixerResponse.Rates)
                            {
                                if (string.Equals(rateKvp.Key, "EUR", StringComparison.OrdinalIgnoreCase))
                                    continue;
                                if (existingCurrencies.TryGetValue(rateKvp.Key, out int targetCurrencyId))
                                {
                                    await uow.ExchangeRateRepository.InsertExchangeRateAsync(baseCurrencyId, targetCurrencyId, dateId, rateKvp.Value);
                                }
                            }
                            _logger.LogInformation("Updated exchange rates for all currencies for {Date}", today.ToShortDateString());
                        }
                    }
                    else
                    {
                        _logger.LogError("Base currency EUR not found in the existing currencies.");
                    }

                    await uow.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in DailyExchangeRateService.");
                    await uow.RollbackAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DailyExchangeRateService is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }