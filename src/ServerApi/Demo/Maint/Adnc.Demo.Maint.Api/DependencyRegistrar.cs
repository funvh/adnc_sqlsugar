using Adnc.Demo.Maint.Repository;
using Adnc.Demo.Shared.Rpc.Http.Services;
using Adnc.Shared.Rpc.Http.Services;
using Adnc.Shared.WebApi.Registrar;
using System.Reflection;

namespace Adnc.Demo.Maint.Api;

public sealed class MaintWebApiDependencyRegistrar : AbstractDependencyRegistrar
{
    public MaintWebApiDependencyRegistrar(IServiceCollection services)
        : base(services)
    {
    }

    public MaintWebApiDependencyRegistrar(IApplicationBuilder app)
    : base(app)
    {
    }

    protected override Assembly ApplicationContractAssembly => Assembly.Load(ServiceInfo.ApplicationContractAssemblyName);

    protected override Assembly ApplicationAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly RepositoryOrDomainAssembly => typeof(EntityInfo).Assembly;

    public override void AddAdnc()
    {
        AddWebApiDefault();
        AddHealthChecks(true, true, true, true);
        Services.AddGrpc();

        //rpc-rest
        var restPolicies = this.GenerateDefaultRefitPolicies();
        AddRestClient<IAuthRestClient>(ServiceAddressConsts.AdncDemoAuthService, restPolicies);
        AddRestClient<IUsrRestClient>(ServiceAddressConsts.AdncDemoUsrService, restPolicies);

        AddRabbitMqClient();
    }

    public override void UseAdnc()
    {
        UseWebApiDefault(endpointRoute: endpoint =>
       {
           endpoint.MapGrpcService<Grpc.MaintGrpcServer>();
       });
    }

    protected override void AddDbContextWithRepositories()
        => Services.AddEfCoreContextWithRepositories(RepositoryOrDomainAssembly, Configuration);
}