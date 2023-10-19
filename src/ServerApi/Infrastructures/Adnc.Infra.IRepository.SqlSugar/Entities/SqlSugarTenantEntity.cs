using Adnc.Infra.IRepository.Entities;

namespace Adnc.Infra.IRepository.SqlSugar.Entities
{
    public abstract class SqlSugarTenantEntity : SqlSugarSoftDeleteEntity, ITenantIdFiter
    {
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnDescription = "创建人")]
        public virtual long Creator { get; set; }

        /// <summary>
        /// 租户
        /// </summary>
        [SugarColumn(ColumnDescription = "租户")]
        public long TenantId { get; set; }
    }
}
