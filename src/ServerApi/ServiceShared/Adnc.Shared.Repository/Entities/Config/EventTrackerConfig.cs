namespace Adnc.Shared.Repository.EfEntities.Config;

public class EventTrackerConfig : AbstractEntityTypeConfiguration<EFEventTracker>
{
    public override void Configure(EntityTypeBuilder<EFEventTracker> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.TrackerName).HasMaxLength(50);
        builder.HasIndex(x => new { x.EventId, x.TrackerName }, "uk_eventid_trackername").IsUnique();
    }
}