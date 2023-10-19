namespace Adnc.Infra.IRepository.Entities
{
    /// <summary>
    /// 租户Id接口过滤器
    /// </summary>
    public interface ITenantIdFiter
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        long TenantId { get; set; }
    }
}
