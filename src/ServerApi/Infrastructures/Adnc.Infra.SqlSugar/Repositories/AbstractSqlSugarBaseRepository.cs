namespace Adnc.Infra.Repository.SqlSugar.Repositories
{
    /// <summary>
    /// SqlSugar 仓储的基类实现,抽象类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="ISqlSugarClient"></typeparam>
    public abstract class AbstractSqlSugarBaseRepository<ISqlSugarClient, TEntity> : ISqlSugarBaseRepository<TEntity>
        where TEntity : Entity, ISqlSugarEntity<long>, new()
    {
        protected readonly IServiceProvider _serviceProvider;

        public virtual SqlSugarScope DbContext { get; }

        protected AbstractSqlSugarBaseRepository(ISqlSugarClient dbContext, IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(nameof(dbContext));

            DbContext = dbContext as SqlSugarScope;
            this._serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 切换仓储
        /// </summary>
        /// <typeparam name="OtherEntity">实体类型</typeparam>
        /// <returns>仓储</returns>
        protected virtual ISqlSugarRepository<OtherEntity> Change<OtherEntity>() where OtherEntity : SqlSugarEntity, new()
        {
            return _serviceProvider.GetService<ISqlSugarRepository<OtherEntity>>();
        }

        ///// <summary>
        ///// 原生 Ado 对象
        ///// </summary>
        //IAdo Ado { get; }

        protected virtual ISugarQueryable<TEntity> GetDbSet(bool writeDb, bool noTracking)
        {
            //TODO:待实现分库查询或写入
            return DbContext.Queryable<TEntity>();
        }

        public virtual ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression, bool writeDb = false, bool noTracking = true)
        {
            return DbContext.Queryable<TEntity>().Where(expression);
        }

        public virtual ISugarQueryable<TEntity> WhereIF(bool isWhere, Expression<Func<TEntity, bool>> expression)
        {
            return DbContext.Queryable<TEntity>().WhereIF(isWhere, expression);
        }

        public virtual Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == 0)
                entity.Id = IdGenerater.Yitter.IdGenerater.GetNextId();

            return DbContext.Insertable(entity).ExecuteCommandAsync(cancellationToken);
        }

        public Task<long> InsertReturnIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return DbContext.Insertable(entity).ExecuteReturnSnowflakeIdAsync(cancellationToken);
        }

        public virtual Task<int> InsertAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return DbContext.Insertable(entities).ExecuteCommandAsync(cancellationToken);
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression, bool writeDb = false, CancellationToken cancellationToken = default)
        {
            return DbContext.Queryable<TEntity>().AnyAsync(whereExpression);
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression, bool writeDb = false, CancellationToken cancellationToken = default)
        {
            return DbContext.Queryable<TEntity>().CountAsync(whereExpression);
        }

        public virtual Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return DbContext.Updateable(entities.ToArray()).ExecuteCommandAsync(cancellationToken);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var changedCount = await DbContext.Updateable(entity).ExecuteCommandAsync();
            return changedCount > 0;
        }

        public Task<TEntity?> GetAsync(long keyValue, Expression<Func<TEntity, dynamic>>? navigationPropertyPath = null, bool writeDb = false, CancellationToken cancellationToken = default)
        {
            return DbContext.CopyNew().Queryable<TEntity>().FirstAsync(f => f.Id == keyValue);
        }

        public Task<List<long>> InsertReturnIdsAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return DbContext.Insertable(entities).ExecuteReturnSnowflakeIdListAsync(cancellationToken);
        }

        public InsertNavTaskInit<TEntity, TEntity> InsertNav(TEntity entity)
        {
            return DbContext.CopyNew().InsertNav(entity);
        }
    }
}