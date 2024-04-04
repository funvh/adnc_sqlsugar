using Adnc.Demo.Cust.Api.Application.Subscribers;
using Adnc.Shared.WebApi.Registrar;
using System.Reflection;

namespace Adnc.Demo.Cust.Api;

public sealed class ApiLayerRegistrar : AbstractDependencyRegistrar
{
    public ApiLayerRegistrar(IServiceCollection services) : base(services)
    {
        _assembly = Assembly.GetExecutingAssembly();
    }

    public ApiLayerRegistrar(IApplicationBuilder appBuilder) : base(appBuilder)
    {
        _assembly = Assembly.GetExecutingAssembly();
    }

    private readonly Assembly _assembly;

    protected override Assembly AppServiceInterfaceLayerAssembly => _assembly;

    protected override Assembly ApplicationAssembly => _assembly;

    protected override Assembly RepositoryOrDomainAssembly => _assembly;

    public override void AddAdnc()
    {
        AddWebApiDefault();
        AddHealthChecks(true, true, true, false);
        //register others services
        //Services.AddScoped<xxxx>


        //register rpc-http services
        var restPolicies = this.GenerateDefaultRefitPolicies();
        AddRestClient<IAuthRestClient>(ServiceAddressConsts.AdncDemoAuthService, restPolicies);
        AddRestClient<IUsrRestClient>(ServiceAddressConsts.AdncDemoUsrService, restPolicies);
        AddRestClient<IMaintRestClient>(ServiceAddressConsts.AdncDemoMaintService, restPolicies);
        AddRestClient<IWhseRestClient>(ServiceAddressConsts.AdncDemoWhseService, restPolicies);

        var gprcPolicies = this.GenerateDefaultGrpcPolicies();
        AddGrpcClient<AuthGrpc.AuthGrpcClient>(ServiceAddressConsts.AdncDemoAuthService, gprcPolicies);
        AddGrpcClient<UsrGrpc.UsrGrpcClient>(ServiceAddressConsts.AdncDemoUsrService, gprcPolicies);
        AddGrpcClient<MaintGrpc.MaintGrpcClient>(ServiceAddressConsts.AdncDemoMaintService, gprcPolicies);
        AddGrpcClient<WhseGrpc.WhseGrpcClient>(ServiceAddressConsts.AdncDemoWhseService, gprcPolicies);

        //register rpc-event service
        AddCapEventBus();
        Services.AddScoped<ICapSubscribe, CapEventSubscriber>();

        //register others services
        // 添加模型验证扫描服务
        Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
    }

    public override void UseAdnc()
    {
        UseWebApiDefault();
    }

    protected override void AddDbContextWithRepositories()
        => Services.AddEfCoreContextWithRepositories(RepositoryOrDomainAssembly, Configuration);
}