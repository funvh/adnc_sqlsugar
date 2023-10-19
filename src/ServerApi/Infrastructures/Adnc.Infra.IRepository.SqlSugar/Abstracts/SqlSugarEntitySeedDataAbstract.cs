using Adnc.Infra.IRepository.SqlSugar.Entities;

namespace Adnc.Infra.IRepository.SqlSugar.Abstracts;

/// <summary>
/// 实体种子数据接口
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public abstract class SqlSugarEntitySeedDataAbstract<TEntity>
    where TEntity : SqlSugarEntity, new()
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<TEntity> HasData();

    /// <summary>
    /// 修改种子数据
    /// </summary>
    /// <returns></returns>
    public virtual void UpdateData(ISqlSugarClient sqlSugarClient)
    {
        var updateList = HasData();

        var db = sqlSugarClient.CopyNew();

        db.QueryFilter.Clear<ISoftDeleteFilter>();

        var needDeleteList = db.Queryable<TEntity>()
            .Where(w => !updateList.Select(s => s.Id).Contains(w.Id))
            .ToList();

        if (needDeleteList.IsNotNullOrEmpty())
            db.Deleteable(needDeleteList).ExecuteCommand();

        var storage = db.Storageable(updateList.ToList()).ToStorage();
        storage.AsInsertable.ExecuteCommand();
        storage.AsUpdateable.ExecuteCommand();
    }
}