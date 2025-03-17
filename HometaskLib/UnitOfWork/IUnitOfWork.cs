using HometaskLib.Repositories;

namespace HometaskLib.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IDateDimensionRepository DateDimensionRepository { get; }
    ICurrencyRepository CurrencyRepository { get; }
    IExchangeRateRepository ExchangeRateRepository { get; }
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}