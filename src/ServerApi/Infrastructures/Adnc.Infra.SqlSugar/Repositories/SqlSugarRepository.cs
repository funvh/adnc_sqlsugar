using DbType = SqlSugar.DbType;

namespace Adnc.Infra.Repository.SqlSugar.Repositories
{
    /// <summary>
    /// SqlSugar默认的、全功能的仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SqlSugarRepository<TEntity> : AbstractSqlSugarBaseRepository<ISqlSugarClient, TEntity>,
        ISqlSugarRepository<TEntity> where TEntity : SqlSugarEntity, new()
    {
        private readonly IAdoQuerierRepository? _adoQuerier;

        public SqlSugarRepository(ISqlSugarClient dbContext,
            IServiceProvider serviceProvider,
            IAdoQuerierRepository? adoQuerier = null)
            : base(dbContext, serviceProvider)
            => _adoQuerier = adoQuerier;

        public IAdoQuerierRepository? AdoQuerier
        {
            get
            {
                if (_adoQuerier is null)
                    return null;
                if (!_adoQuerier.HasDbConnection())
                {
                    DbTypes? dbType = null;
                    switch (DbContext.CurrentConnectionConfig.DbType)
                    {
                        case DbType.MySql:
                            dbType = DbTypes.MYSQL;
                            break;
                        case DbType.SqlServer:
                            dbType = DbTypes.SQLSERVER;
                            break;
                        case DbType.Oracle:
                            dbType = DbTypes.ORACLE;
                            break;
                    }

                    ArgumentNullException.ThrowIfNull(nameof(dbType));

                    _adoQuerier.ChangeOrSetDbConnection(DbContext.CurrentConnectionConfig.ConnectionString, dbType.Value);
                }
                return _adoQuerier;
            }
        }

        public IDbTransaction? CurrentDbTransaction => DbContext.Ado.Transaction;

        //public async Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql, CancellationToken cancellationToken = default) =>
        //   await DbContext.Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);

        public async Task<int> ExecuteSqlWriteAsync(string sql, object param, CancellationToken cancellationToken = default) =>
           await DbContext.Ado.ExecuteCommandAsync(sql, param, cancellationToken);

        //public IDbTransaction? CurrentDbTransaction => DbContext.Database.CurrentTransaction?.GetDbTransaction();

        public virtual ISugarQueryable<TEntity> AsQueryable() => DbContext.Queryable<TEntity>();

        //public virtual IQueryable<TrdEntity> GetAll<TrdEntity>(bool writeDb = false, bool noTracking = true)
        //       where TrdEntity : EfEntity
        //{
        //    var queryAble = DbContext.Set<TrdEntity>().AsQueryable();
        //    if (writeDb)
        //        queryAble = queryAble.TagWith(RepositoryConsts.MAXSCALE_ROUTE_TO_MASTER);
        //    if (noTracking)
        //        queryAble = queryAble.AsNoTracking();
        //    return queryAble;
        //}

        public virtual async Task<TEntity?> FindAsync(long keyValue, Expression<Func<TEntity, dynamic>>? navigationPropertyPath = null, bool writeDb = false, bool noTracking = true, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Queryable<TEntity>().Where(t => t.Id == keyValue);

            if (navigationPropertyPath is not null)
                return await query.Includes(navigationPropertyPath).FirstAsync(cancellationToken);
            else
                return await query.FirstAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, dynamic>>? navigationPropertyPath = null, Expression<Func<TEntity, object>>? orderByExpression = null, bool ascending = false, bool writeDb = false, bool noTracking = true, CancellationToken cancellationToken = default)
        {
            TEntity? result;

            var query = GetDbSet(writeDb, noTracking).Where(whereExpression);

            if (navigationPropertyPath is not null)
                query = query.Includes(navigationPropertyPath);

            if (orderByExpression is null)
                result = await query.OrderByDescending(x => x.Id).FirstAsync(cancellationToken);
            else
                result = ascending
                          ? await query.OrderBy(orderByExpression).FirstAsync(cancellationToken)
                          : await query.OrderByDescending(orderByExpression).FirstAsync(cancellationToken)
                          ;

            return result;
        }

        public virtual async Task<TResult?> FetchAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>>? orderByExpression = null, bool ascending = false, bool writeDb = false, bool noTracking = true, CancellationToken cancellationToken = default)
        {
            TResult? result;

            var query = this.GetDbSet(writeDb, noTracking).Where(whereExpression);

            if (orderByExpression is null)
                result = await query.OrderByDescending(x => x.Id).Select(selector).FirstAsync(cancellationToken);
            else
                result = ascending
                          ? await query.OrderBy(orderByExpression).Select(selector).FirstAsync(cancellationToken)
                          : await query.OrderByDescending(orderByExpression).Select(selector).FirstAsync(cancellationToken);

            return result;
        }

        public virtual async Task<bool> DeleteAsync(long keyValue, bool isSoftDelete = true, CancellationToken cancellationToken = default)
        {
            //如果是软删除（修改实体的IsDelete状态）
            if (isSoftDelete)
            {
                //判断该实体类是否继承了ISoftDelete
                if (typeof(TEntity).IsAssignableTo(typeof(ISoftDeleteFilter)))
                {
                    return await DbContext.Updateable<TEntity>().Where(w => w.Id == keyValue).SetColumns("IsDeleted", true).ExecuteCommandAsync() > 0;
                }
            }

            return await DbContext.Deleteable<TEntity>().Where(w => w.Id == keyValue).ExecuteCommandAsync() > 0;
        }

        //public virtual async Task<bool> DeleteAsync(long keyValue, bool isSoftDelete = true, CancellationToken cancellationToken = default)
        //{
        //    Type entityType = typeof(TEntity);

        //    var properties = entityType.GetProperties();

        //    //需要删除的导航属性
        //    var needDeleteProperties = properties.Where(w => w.IsDefined(typeof(NeedDeleteNavigationAttribute), false)).ToList();

        //    //如果是软删除（修改实体的IsDelete状态）
        //    if (isSoftDelete)
        //    {
        //        //判断该实体类是否继承了ISoftDelete
        //        if (entityType.IsAssignableTo(typeof(ISoftDelete)))
        //        {
        //            //如果有需要删除的导航属性
        //            if (needDeleteProperties.Count > 0)
        //            {
        //                var entity = await DbContext.Queryable<TEntity>().FirstAsync(f => f.Id == keyValue);

        //            }

        //            return await DbContext.Updateable<TEntity>().Where(w => w.Id == keyValue).SetColumns("IsDeleted", true).ExecuteCommandAsync() > 0;
        //        }
        //    }

        //    return await DbContext.Deleteable<TEntity>().Where(w => w.Id == keyValue).ExecuteCommandAsync() > 0;
        //}

        public virtual async Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            var enityType = typeof(TEntity);
            var hasSoftDeleteMember = typeof(ISoftDeleteFilter).IsAssignableFrom(enityType);
            if (hasSoftDeleteMember)
            {
                return await DbContext.Updateable<TEntity>()
                    .Where(whereExpression)
                    .SetColumns("IsDeleted", true)
                    .SetColumns("ModifiedTime", DateTime.Now)
                    .ExecuteCommandAsync(cancellationToken);
            }

            return await DbContext.Deleteable<TEntity>().Where(whereExpression).ExecuteCommandAsync(cancellationToken);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>>[] updatingExpressions, CancellationToken cancellationToken = default)
        {
            if (updatingExpressions.IsNullOrEmpty())
                return await UpdateAsync(entity, cancellationToken);
            else
                return await DbContext.Updateable<TEntity>(updatingExpressions).ExecuteCommandAsync(cancellationToken) > 0;
        }

        public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> queryExpression, CancellationToken cancellationToken = default)
        {
            return DbContext.Queryable<TEntity>().FirstAsync(queryExpression, cancellationToken);
        }

        public NavISugarQueryable<TEntity> AsNavQueryable() => DbContext.Queryable<TEntity>().AsNavQueryable();

        public IUpdateable<TEntity> AsUpdateable() => DbContext.Updateable<TEntity>();

        public IUpdateable<TEntity> AsUpdateable(dynamic updateDynamicObject) => DbContext.Updateable<TEntity>(updateDynamicObject);

        public UpdateNavTaskInit<TEntity, TEntity> AsNavUpdateable(dynamic updateDynamicObject) => DbContext.UpdateNav<TEntity>(updateDynamicObject);

        public virtual Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TEntity>> updatingExpression, CancellationToken cancellationToken = default)
        {
            var enityType = typeof(TEntity);
            var hasConcurrencyMember = typeof(IConcurrency).IsAssignableFrom(enityType);

            if (hasConcurrencyMember)
                throw new ArgumentException("该实体有RowVersion列，不能使用批量更新");

            return UpdateRangeInternalAsync(whereExpression, updatingExpression, cancellationToken);
        }

        //public virtual async Task<int> UpdateRangeAsync(Dictionary<long, List<(string propertyName, dynamic propertyValue)>> propertyNameAndValues, CancellationToken cancellationToken = default)
        //{
        //    var existsEntities = DbContext.Set<TEntity>().Local.Where(x => propertyNameAndValues.ContainsKey(x.Id));

        //    foreach (var item in propertyNameAndValues)
        //    {
        //        var enity = existsEntities?.FirstOrDefault(x => x.Id == item.Key) ?? new TEntity { Id = item.Key };
        //        var entry = DbContext.Entry(enity);
        //        if (entry.State == EntityState.Detached)
        //            entry.State = EntityState.Unchanged;

        //        if (entry.State == EntityState.Unchanged)
        //        {
        //            var info = propertyNameAndValues.FirstOrDefault(x => x.Key == item.Key).Value;
        //            info.ForEach(x =>
        //            {
        //                entry.Property(x.propertyName).CurrentValue = x.propertyValue;
        //                entry.Property(x.propertyName).IsModified = true;
        //            });
        //        }
        //    }

        //    return await DbContext.SaveChangesAsync(cancellationToken);
        //}

        protected virtual async Task<int> UpdateRangeInternalAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TEntity>> updatingExpression, CancellationToken cancellationToken = default)
        {
            return await DbContext.Updateable<TEntity>().SetColumns(updatingExpression).Where(whereExpression).ExecuteCommandAsync();
        }
    }
}