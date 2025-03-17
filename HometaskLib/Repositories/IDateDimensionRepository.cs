using Microsoft.Data.SqlClient;

namespace HometaskLib.Repositories;

public interface IDateDimensionRepository
{
    Task<bool> DateExistsAsync(DateTime date);
    Task<int> InsertDateDimensionAsync(DateTime date);
    void SetTransaction(SqlTransaction transaction);
}