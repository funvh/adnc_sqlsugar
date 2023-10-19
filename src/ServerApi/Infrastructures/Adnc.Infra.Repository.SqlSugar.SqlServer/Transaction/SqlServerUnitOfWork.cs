using SqlSugar;

namespace Adnc.Infra.Repository.SqlSugar.SqlServer;

public class SqlSugarSqlServerUnitOfWork : UnitOfWork
{
    private ICapPublisher? _publisher;

    public SqlSugarSqlServerUnitOfWork(
        ISqlSugarClient context,
        ICapPublisher? publisher = null)
        : base(context)
    {
        _publisher = publisher;
    }
}