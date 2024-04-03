using Adnc.Infra.IRepository.EFCore;
using Adnc.Shared.Repository.EFCore;

namespace Adnc.Demo.Cust.Api.Repository.Entities;

public class EntityInfo : AbstracSharedEntityInfo
{
    public EntityInfo(UserContext userContext) : base(userContext)
    {
    }

    protected override Assembly GetCurrentAssembly() => GetType().Assembly;

    protected override void SetTableName(dynamic modelBuilder)
    {
        if (modelBuilder is not ModelBuilder builder)
            throw new ArgumentNullException(nameof(modelBuilder));

        builder.Entity<EFEventTracker>().ToTable("cust_eventtracker");
    }
}