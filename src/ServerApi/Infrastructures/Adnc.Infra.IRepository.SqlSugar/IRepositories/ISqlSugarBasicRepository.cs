using Adnc.Infra.IRepository.SqlSugar.Entities;

namespace Adnc.Infra.IRepository.SqlSugar;

/// <summary>
/// Ef简单的、基础的，初级的仓储接口
/// 适合DDD开发模式,实体必须继承AggregateRoot
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface ISqlSugarBasicRepository<TEntity> : ISqlSugarBaseRepository<TEntity>
    where TEntity : Entity, ISqlSugarEntity<long>, new()
{
    //Task<int> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    //Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}