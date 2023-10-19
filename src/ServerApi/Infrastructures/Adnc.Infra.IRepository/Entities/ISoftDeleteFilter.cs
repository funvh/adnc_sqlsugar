namespace Adnc.Infra.IRepository
{
    /// <summary>
    /// 软删除过滤
    /// </summary>
    public interface ISoftDeleteFilter
    {
        bool IsDeleted { get; set; }
    }
}