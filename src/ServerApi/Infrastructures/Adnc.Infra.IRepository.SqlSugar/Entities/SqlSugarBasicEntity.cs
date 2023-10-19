namespace Adnc.Infra.IRepository.SqlSugar.Entities
{
    public abstract class SqlSugarBasicEntity : SqlSugarEntity, IBasicEntity
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDataType = "datetime", InsertServerTime = true, IsOnlyIgnoreUpdate = true)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnDataType = "datetime", UpdateServerTime = true, IsOnlyIgnoreInsert = true)]
        public DateTime? ModifiedTime { get; set; }
    }
}
