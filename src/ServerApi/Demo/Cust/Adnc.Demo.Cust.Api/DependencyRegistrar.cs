using Adnc.Demo.Cust.Api.Application.Subscribers;
using Adnc.Shared.WebApi.Registrar;

namespace Adnc.Demo.Cust.Api;

public sealed class ApiLayerRegistrar : AbstractDependencyRegistrar
{
    public ApiLayerRegistrar(IServiceCollection services) : base(services)
    {
    }

    public ApiLayerRegistrar(IApplicationBuilder appBuilder) : base(appBuilder)
    {
    }

    protected override Assembly ApplicationContractAssembly => Assembly.GetExecutingAssembly();

    protected override Assembly ApplicationAssembly => Assembly.GetExecutingAssembly();

    protected override Assembly RepositoryOrDomainAssembly => Assembly.GetExecutingAssembly();

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
    }

    public override void UseAdnc()
    {
        UseWebApiDefault();
    }

    protected override void AddDbContextWithRepositories()
        => Services.AddEfCoreContextWithRepositories(RepositoryOrDomainAssembly, Configuration);
}