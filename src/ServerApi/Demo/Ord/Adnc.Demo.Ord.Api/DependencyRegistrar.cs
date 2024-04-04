using Adnc.Demo.Ord.Application.EventSubscribers;
using Adnc.Demo.Ord.Domain.EntityConfig;
using Adnc.Demo.Shared.Const;
using Adnc.Demo.Shared.Rpc.Http.Services;
using Adnc.Shared.Domain;
using Adnc.Shared.Rpc.Http.Services;
using Adnc.Shared.WebApi.Registrar;
using DotNetCore.CAP;
using FluentValidation;
using System.Reflection;

namespace Adnc.Demo.Ord.Api;

public sealed class OrdWebApiDependencyRegistrar : AbstractDependencyRegistrar
{
    protected override Assembly AppServiceInterfaceLayerAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly ApplicationAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly RepositoryOrDomainAssembly => Assembly.Load(ServiceInfo.DomainAssemblyName);

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

        // 添加模型验证扫描服务
        Services.AddValidatorsFromAssembly(ApplicationAssembly, ServiceLifetime.Scoped);
    }

    public override void UseAdnc()
    {
        UseWebApiDefault();
    }

    protected override void AddDbContextWithRepositories()
        => Services.AddEfCoreContextWithRepositories(RepositoryOrDomainAssembly, Configuration);
}