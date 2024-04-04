using Adnc.Infra.IRepository.SqlSugar;
using Adnc.Infra.IRepository.SqlSugar.Entities;

namespace Adnc.Shared.Application.Services.Trackers;

public class SqlSugarMessageTrackerService : IMessageTracker
{
    public ISqlSugarRepository<SqlSugarEventTracker> _trackerRepo;

    public SqlSugarMessageTrackerService(ISqlSugarRepository<SqlSugarEventTracker> trackerRepo)
    {
        _trackerRepo = trackerRepo;
    }

    public TrackerKind Kind => TrackerKind.Db;

    public async Task<bool> HasProcessedAsync(long eventId, string trackerName)
    {
        return await _trackerRepo.AnyAsync(x => x.EventId == eventId && x.TrackerName == trackerName, true);
    }

    public async Task MarkAsProcessedAsync(long eventId, string trackerName)
    {
        await _trackerRepo.InsertAsync(new SqlSugarEventTracker
        {
            Id = IdGenerater.GetNextId(),
            EventId = eventId,
            TrackerName = trackerName
        });
    }
}
