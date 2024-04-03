using Adnc.Infra.IRepository.EFCore;

namespace Adnc.Shared.Application.Services.Trackers;

public class DbMessageTrackerService : IMessageTracker
{
    public IEfRepository<EFEventTracker> _trackerRepo;

    public DbMessageTrackerService(IEfRepository<EFEventTracker> trackerRepo)
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
        await _trackerRepo.InsertAsync(new EFEventTracker
        {
            Id = IdGenerater.GetNextId(),
            EventId = eventId,
            TrackerName = trackerName
        });
    }
}
