using Adnc.Infra.Repository.EfCore.MySql.Configurations;
using Adnc.Shared.Application.Registrar;
using Adnc.Shared.Rpc.Http.Services;
using Microsoft.Extensions.Configuration;

namespace Adnc.Demo.Ord.Application;

public sealed class OrdApplicationDependencyRegistrar : AbstractApplicationDependencyRegistrar
{
    public override Assembly ApplicationLayerAssembly => Assembly.GetExecutingAssembly();

    public override Assembly ContractsLayerAssembly => typeof(IOrderAppService).Assembly;

    public override Assembly RepositoryOrDomainLayerAssembly => typeof(EntityInfo).Assembly;

    public OrdApplicationDependencyRegistrar(IServiceCollection services) : base(services)
    {
    }

    public override void AddAdnc()
    {
        base.AddAdnc();

        AddDomainSerivces<IDomainService>();

        //rpc-rest
        var restPolicies = PollyStrategyEnable ? this.GenerateDefaultRefitPolicies() : new();
        AddRestClient<IAuthRestClient>(ServiceAddressConsts.AdncDemoAuthService, restPolicies);
        AddRestClient<IUsrRestClient>(ServiceAddressConsts.AdncDemoUsrService, restPolicies);
        AddRestClient<IMaintRestClient>(ServiceAddressConsts.AdncDemoMaintService, restPolicies);
        AddRestClient<IWhseRestClient>(ServiceAddressConsts.AdncDemoWhseService, restPolicies);

        //rpc-event
        AddCapEventBus();
        Services.AddScoped<ICapSubscribe, CapEventSubscriber>();
    }

    public override void AddAdncInfraMapper()
        => Services.AddAutoMapper(ApplicationLayerAssembly);

    protected override void AddDbContextWithRepositories()
       => Services.AddMySqlDbContextAndRepository(MysqlSection, RepositoryOrDomainLayerAssembly);
}