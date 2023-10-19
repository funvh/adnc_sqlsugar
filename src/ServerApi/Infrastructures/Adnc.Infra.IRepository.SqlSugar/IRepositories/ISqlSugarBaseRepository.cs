using Adnc.Infra.IRepository.SqlSugar.Entities;

namespace Adnc.Infra.IRepository.SqlSugar;

/// <summary>
/// SqlSugar仓储的基类接口
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface ISqlSugarBaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : Entity, ISqlSugarEntity<long>, new()
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public SqlSugarScope DbContext { get; }

    /// <summary>
    /// 插入实体，自动生成雪花Id
    /// </summary>
    /// <param name="entity"><see cref="T:TEntity"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<long> InsertReturnIdAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导航插入
    /// </summary>
    /// <param name="entity"><see cref="T:TEntity"/></param>
    /// <returns></returns>
    InsertNavTaskInit<TEntity, TEntity> InsertNav(TEntity entity);

    /// <summary>
    /// 插入实体
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量插入实体
    /// </summary>
    /// <param name="entities"><see cref="T:TEntity"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<int> InsertAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量插入实体，并返回Id集合
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<long>> InsertReturnIdsAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新单个实体
    /// </summary>
    /// <param name="entity"><see cref="T:TEntity"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查询实体是否存在
    /// </summary>
    /// <param name="whereExpression">查询条件</param>
    /// <param name="writeDb">是否读写库，默认false,可选参数</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression, bool writeDb = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 统计符合条件的实体数量
    /// </summary>
    /// <param name="whereExpression">查询条件</param>
    /// <param name="writeDb">是否读写库，默认false,可选参数</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression, bool writeDb = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查询，返回IQueryable{TEntity}
    /// </summary>
    /// <param name="expression">查询条件</param>
    /// <param name="writeDb">是否读写库，默认false,可选参数</param>
    /// <param name="noTracking">是否开启跟踪，默认false,可选参数</param>
    /// <returns></returns>
    ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression, bool writeDb = false, bool noTracking = true);

    /// <summary>
    /// 根据条件查询，返回IQueryable{TEntity}
    /// </summary>
    /// <param name="isWhere"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    ISugarQueryable<TEntity> WhereIF(bool isWhere, Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// 获取单个实体
    /// </summary>
    /// <param name="keyValue"></param>
    /// <param name="navigationPropertyPath"></param>
    /// <param name="writeDb"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> GetAsync(long keyValue, Expression<Func<TEntity, dynamic>>? navigationPropertyPath = null, bool writeDb = false, CancellationToken cancellationToken = default);
}