namespace Adnc.Infra.Repository.SqlSugar.Transaction;

public abstract class UnitOfWork : IUnitOfWork
{
    protected ISqlSugarClient DbContext { get; set; }

    public bool IsStartingUow => DbContext.Ado.IsAnyTran();

    protected UnitOfWork(ISqlSugarClient context)
    {
        DbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distributed = false)
    {
        DbContext.Ado.BeginTran();
    }

    public void Commit()
    {
        DbContext.Ado.CommitTran();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await DbContext.Ado.CommitTranAsync();
    }

    public void Rollback()
    {
        DbContext.Ado.RollbackTran();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await DbContext.Ado.RollbackTranAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (DbContext is not null)
            {
                DbContext.Dispose();
                DbContext = null;
            }
        }
    }
}