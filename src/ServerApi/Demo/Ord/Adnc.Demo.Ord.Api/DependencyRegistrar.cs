using Adnc.Demo.Ord.Application.EventSubscribers;
using Adnc.Demo.Ord.Domain.EntityConfig;
using Adnc.Demo.Shared.Const;
using Adnc.Demo.Shared.Rpc.Http.Services;
using Adnc.Shared.Domain;
using Adnc.Shared.Rpc.Http.Services;
using Adnc.Shared.WebApi.Registrar;
using DotNetCore.CAP;
using System.Reflection;

namespace Adnc.Demo.Ord.Api;

public sealed class OrdWebApiDependencyRegistrar : AbstractDependencyRegistrar
{
    protected override Assembly ApplicationContractAssembly => Assembly.Load(ServiceInfo.ApplicationContractAssemblyName);

    protected override Assembly ApplicationAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly RepositoryOrDomainAssembly => typeof(EntityInfo).Assembly;

    public OrdWebApiDependencyRegistrar(IServiceCollection services)
        : base(services)
    {
    }

    public OrdWebApiDependencyRegistrar(IApplicationBuilder app)
    : base(app)
    {
    }

    public override void AddAdnc()
    {
        AddWebApiDefault();
        AddHealthChecks(true, true, true, true);


        AddDomainSerivces<IDomainService>();

        //rpc-rest
        var restPolicies = this.GenerateDefaultRefitPolicies();
        AddRestClient<IAuthRestClient>(ServiceAddressConsts.AdncDemoAuthService, restPolicies);
        AddRestClient<IUsrRestClient>(ServiceAddressConsts.AdncDemoUsrService, restPolicies);
        AddRestClient<IMaintRestClient>(ServiceAddressConsts.AdncDemoMaintService, restPolicies);
        AddRestClient<IWhseRestClient>(ServiceAddressConsts.AdncDemoWhseService, restPolicies);

        //rpc-event
        AddCapEventBus();
        Services.AddScoped<ICapSubscribe, CapEventSubscriber>();
    }

    public override void UseAdnc()
    {
        UseWebApiDefault();
    }

    protected override void AddDbContextWithRepositories()
        => Services.AddEfCoreContextWithRepositories(RepositoryOrDomainAssembly, Configuration);
}