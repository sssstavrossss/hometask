using Microsoft.Data.SqlClient;

namespace HometaskLib.Repositories;

public interface ICurrencyRepository
{
    Task<Dictionary<string, int>> GetExistingCurrenciesAsync();
    Task<int> InsertCurrencyAsync(string currencyCode, string currencyName);
    void SetTransaction(SqlTransaction transaction);
}