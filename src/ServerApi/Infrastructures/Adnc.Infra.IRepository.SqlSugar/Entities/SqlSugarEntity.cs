namespace Adnc.Infra.IRepository.SqlSugar.Entities
{
    public abstract class SqlSugarEntity : Entity, ISqlSugarEntity<long>
    {
        /// <summary>
        /// 雪花Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public override long Id { get; set; }
    }
}