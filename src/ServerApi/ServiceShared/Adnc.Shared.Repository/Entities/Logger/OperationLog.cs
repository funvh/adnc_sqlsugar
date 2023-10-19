﻿using Adnc.Infra.IRepository.SqlSugar.Entities;
using SqlSugar;

namespace Adnc.Shared.Repository.Entities.Logger;

/// <summary>
/// 操作日志
/// </summary>
//public class OperationLog : MongoEntity
[SugarTable("dt_log_operation")]
public class OperationLog : SqlSugarEntity
{
    public string ClassName { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime? CreateTime { get; set; }

    public string LogName { get; set; } = default!;

    public string LogType { get; set; } = default!;

    public string Message { get; set; } = default!;

    public string Method { get; set; } = default!;

    public string Succeed { get; set; } = default!;

    public long? UserId { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string RemoteIpAddress { get; set; } = default!;
}