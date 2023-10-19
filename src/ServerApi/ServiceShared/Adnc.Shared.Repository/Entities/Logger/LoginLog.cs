using Adnc.Infra.IRepository.SqlSugar.Entities;
using SqlSugar;

namespace Adnc.Shared.Repository.Entities.Logger;

/// <summary>
/// 登录日志
/// </summary>
[SugarTable("dt_log_login")]
public class LoginLog : SqlSugarEntity
{
    public string Device { get; set; } = default!;

    public string Message { get; set; } = default!;

    public bool Succeed { get; set; } = default!;

    public int StatusCode { get; set; } = default!;

    public long? UserId { get; set; } = default!;

    public string Account { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string RemoteIpAddress { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime? CreateTime { get; set; }
}