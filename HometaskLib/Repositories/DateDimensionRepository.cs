using Microsoft.Data.SqlClient;

namespace HometaskLib.Repositories;

public class DateDimensionRepository : IDateDimensionRepository
{
    private readonly SqlConnection _connection;
    private SqlTransaction _transaction;

    public DateDimensionRepository(SqlConnection connection)
    {
        _connection = connection;
    }

    public void SetTransaction(SqlTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task<bool> DateExistsAsync(DateTime date)
    {
        try
        {
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = "SELECT COUNT(*) FROM DateDimension WHERE CAST(DateValue AS DATE) = @DateValue";
                command.Parameters.AddWithValue("@DateValue", date);
                int count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<int> InsertDateDimensionAsync(DateTime date)
    {
        try
        {
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"
                    INSERT INTO DateDimension (DateValue, Year, Month, Day)
                    VALUES (@DateValue, @Year, @Month, @Day);
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                command.Parameters.AddWithValue("@DateValue", date);
                command.Parameters.AddWithValue("@Year", date.Year);
                command.Parameters.AddWithValue("@Month", date.Month);
                command.Parameters.AddWithValue("@Day", date.Day);
                int dateId = Convert.ToInt32(await command.ExecuteScalarAsync());
                return dateId;
            }
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
}