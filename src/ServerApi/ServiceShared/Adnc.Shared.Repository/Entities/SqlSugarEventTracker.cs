using Adnc.Infra.IRepository.SqlSugar.Entities;
using SqlSugar;

namespace Adnc.Shared.Repository.EfEntities;

/// <summary>
/// 事件跟踪/处理信息
/// </summary>
/// <remarks>
/// EventId,TrackerName 需要建联合唯一索引
/// CREATE UNIQUE INDEX UK_EventId_TrackerNam ON EventTracker(EventId, TrackerName);
/// </remarks>
[NewComDocDatabase]
[SugarTable("SqlSugarEventTracker")]
public class SqlSugarEventTracker : SqlSugarEntity
{
    public long EventId { get; set; }

    public string TrackerName { get; set; } = string.Empty;
}