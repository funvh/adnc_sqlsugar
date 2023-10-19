namespace Adnc.Infra.EventBus;

/// <summary>
/// CapConfig配置
/// </summary>
public class CapConfigOptions
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 数据库类型
    /// </summary>
    public string DbType { get; set; }

    /// <summary>
    /// 消息队列类型
    /// </summary>
    public string MqType { get; set; }
}