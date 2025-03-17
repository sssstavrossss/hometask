namespace HometaskLib.UnitOfWork;
public interface IUnitOfWorkFactory
{
    IUnitOfWork CreateUnitOfWork();
}