using Microsoft.Data.SqlClient;

namespace HometaskLib.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly SqlConnection _connection;
    private SqlTransaction _transaction;

    public CurrencyRepository(SqlConnection connection)
    {
        _connection = connection;
    }

    public void SetTransaction(SqlTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task<Dictionary<string, int>> GetExistingCurrenciesAsync()
    {
        var currencies = new Dictionary<string, int>();
        try
        {
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = "SELECT CurrencyID, CurrencyCode FROM Currency";
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        currencies.Add(reader.GetString(1), reader.GetInt32(0));
                    }
                }
            }

            return currencies;
        }
        catch (Exception ex)
        {
            // Logging
            return currencies;
        }
    }

    public async Task<int> InsertCurrencyAsync(string currencyCode, string currencyName)
    {
        try
        {
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"
                    INSERT INTO Currency (CurrencyCode, CurrencyName)
                    VALUES (@CurrencyCode, @CurrencyName);
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                command.Parameters.AddWithValue("@CurrencyCode", currencyCode);
                command.Parameters.AddWithValue("@CurrencyName", currencyName);
                int currencyId = Convert.ToInt32(await command.ExecuteScalarAsync());
                return currencyId;
            }
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
}