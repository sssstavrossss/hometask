using Microsoft.Data.SqlClient;

namespace HometaskLib.Repositories;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly SqlConnection _connection;
    private SqlTransaction _transaction;

    public ExchangeRateRepository(SqlConnection connection)
    {
        _connection = connection;
    }

    public void SetTransaction(SqlTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task InsertExchangeRateAsync(int baseCurrencyId, int targetCurrencyId, int dateId, decimal rate)
    {
        try
        {
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"
                    INSERT INTO ExchangeRate (BaseCurrencyID, TargetCurrencyID, DateID, Rate)
                    VALUES (@BaseCurrencyID, @TargetCurrencyID, @DateID, @Rate)";
                command.Parameters.AddWithValue("@BaseCurrencyID", baseCurrencyId);
                command.Parameters.AddWithValue("@TargetCurrencyID", targetCurrencyId);
                command.Parameters.AddWithValue("@DateID", dateId);
                command.Parameters.AddWithValue("@Rate", rate);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            
        }
    }
    
    public async Task<decimal?> GetRateAsync(int baseCurrencyId, int targetCurrencyId, DateTime? date)
        {
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = _transaction;
                if (date.HasValue)
                {
                    // When a date is provided, look up the rate for that date.
                    command.CommandText = @"
                        SELECT TOP 1 Rate 
                        FROM ExchangeRate 
                        WHERE BaseCurrencyID = @baseCurrencyId 
                          AND TargetCurrencyID = @targetCurrencyId 
                          AND DateID = (
                                SELECT DateID FROM DateDimension WHERE CAST(DateValue AS DATE) = @DateValue
                          )
                        ORDER BY DateID DESC";
                    command.Parameters.AddWithValue("@baseCurrencyId", baseCurrencyId);
                    command.Parameters.AddWithValue("@targetCurrencyId", targetCurrencyId);
                    command.Parameters.AddWithValue("@DateValue", date.Value.Date);
                }
                else
                {
                    // Otherwise, get the latest available rate.
                    command.CommandText = @"
                        SELECT TOP 1 Rate 
                        FROM ExchangeRate 
                        WHERE BaseCurrencyID = @baseCurrencyId 
                          AND TargetCurrencyID = @targetCurrencyId 
                        ORDER BY DateID DESC";
                    command.Parameters.AddWithValue("@baseCurrencyId", baseCurrencyId);
                    command.Parameters.AddWithValue("@targetCurrencyId", targetCurrencyId);
                }

                var result = await command.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return (decimal)result;
                }
                return null;
            }
        }
}