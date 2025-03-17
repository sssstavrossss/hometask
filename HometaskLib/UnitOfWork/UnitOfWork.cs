using HometaskLib.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HometaskLib.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly SqlConnection _connection;
    private SqlTransaction _transaction;

    public IDateDimensionRepository DateDimensionRepository { get; }
    public ICurrencyRepository CurrencyRepository { get; }
    public IExchangeRateRepository ExchangeRateRepository { get; }

    public UnitOfWork(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SQL");
        _connection = new SqlConnection(connectionString);
            
        // Instantiate repositories with the shared connection.
        DateDimensionRepository = new DateDimensionRepository(_connection);
        CurrencyRepository = new CurrencyRepository(_connection);
        ExchangeRateRepository = new ExchangeRateRepository(_connection);
    }

    public async Task BeginTransactionAsync()
    {
        await _connection.OpenAsync();
        _transaction = _connection.BeginTransaction();
        DateDimensionRepository.SetTransaction(_transaction);
        CurrencyRepository.SetTransaction(_transaction);
        ExchangeRateRepository.SetTransaction(_transaction);
    }

    public async Task CommitAsync()
    {
        _transaction?.Commit();
        await _connection.CloseAsync();
    }

    public async Task RollbackAsync()
    {
        _transaction?.Rollback();
        await _connection.CloseAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}