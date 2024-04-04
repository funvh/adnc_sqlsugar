using Adnc.Demo.Shared.Const;
using Adnc.Demo.Shared.Rpc.Http.Services;
using Adnc.Demo.Whse.Application.Subscribers;
using Adnc.Demo.Whse.Domain.EntityConfig;
using Adnc.Shared.Domain;
using Adnc.Shared.Rpc.Http.Services;
using Adnc.Shared.WebApi.Registrar;
using DotNetCore.CAP;
using System.Reflection;

namespace Adnc.Demo.Whse.Api;

public sealed class WhseWebApiDependencyRegistrar : AbstractDependencyRegistrar
{
    public WhseWebApiDependencyRegistrar(IServiceCollection services)
        : base(services)
    {
    }

    public WhseWebApiDependencyRegistrar(IApplicationBuilder app)
        : base(app)
    {
    }

    protected override Assembly ApplicationContractAssembly => Assembly.Load(ServiceInfo.ApplicationContractAssemblyName);

    protected override Assembly ApplicationAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly RepositoryOrDomainAssembly => typeof(EntityInfo).Assembly;

    public override void AddAdnc()
    {
        AddWebApiDefault();

        var connectionString = Configuration.GetValue<string>("SqlServer:ConnectionString");
        AddHealthChecks(false, true, true, true).AddSqlServer(connectionString);

        Services.AddGrpc();

        AddDomainSerivces<IDomainService>();

        //rpc-rest
        var restPolicies = this.GenerateDefaultRefitPolicies();
        AddRestClient<IAuthRestClient>(ServiceAddressConsts.AdncDemoAuthService, restPolicies);
        AddRestClient<IUsrRestClient>(ServiceAddressConsts.AdncDemoUsrService, restPolicies);
        AddRestClient<IMaintRestClient>(ServiceAddressConsts.AdncDemoMaintService, restPolicies);

        //rpc-event
        AddCapEventBus();
        Services.AddScoped<ICapSubscribe, CapEventSubscriber>();
    }

    public override void UseAdnc()
    {
        UseWebApiDefault(endpointRoute: endpoint =>
        {
            endpoint.MapGrpcService<Grpc.WhseGrpcServer>();
        });
    }

    protected override void AddDbContextWithRepositories()
        => Services.AddEfCoreContextWithRepositories(RepositoryOrDomainAssembly, Configuration);
}