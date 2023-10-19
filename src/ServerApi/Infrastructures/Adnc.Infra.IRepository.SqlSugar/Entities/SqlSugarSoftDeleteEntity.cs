namespace Adnc.Infra.IRepository.SqlSugar.Entities
{
    /// <summary>
    /// 带有软删除字段的实体
    /// </summary>
    public abstract class SqlSugarSoftDeleteEntity : SqlSugarBasicEntity, ISoftDeleteFilter
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnDataType = "bit", ColumnDescription = "是否删除")]
        public bool IsDeleted { get; set; }
    }
}
