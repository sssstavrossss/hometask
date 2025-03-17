using Microsoft.Data.SqlClient;

namespace HometaskLib.Repositories;

public interface IExchangeRateRepository
{
    Task InsertExchangeRateAsync(int baseCurrencyId, int targetCurrencyId, int dateId, decimal rate);
    Task<decimal?> GetRateAsync(int baseCurrencyId, int targetCurrencyId, DateTime? date);
    void SetTransaction(SqlTransaction transaction);
}