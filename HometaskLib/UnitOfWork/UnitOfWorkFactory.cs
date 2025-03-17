using Microsoft.Extensions.Configuration;

namespace HometaskLib.UnitOfWork;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IConfiguration _configuration;

    public UnitOfWorkFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        return new UnitOfWork(_configuration);
    }
}