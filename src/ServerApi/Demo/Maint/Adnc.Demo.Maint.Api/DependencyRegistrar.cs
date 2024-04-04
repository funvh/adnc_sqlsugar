using Adnc.Demo.Maint.Repository;
using Adnc.Demo.Shared.Rpc.Http.Services;
using Adnc.Shared.Rpc.Http.Services;
using Adnc.Shared.WebApi.Registrar;
using FluentValidation;
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

    protected override Assembly AppServiceInterfaceLayerAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly ApplicationAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly RepositoryOrDomainAssembly => Assembly.Load(ServiceInfo.RepositoryAssemblyName);

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

        // 添加模型验证扫描服务
        Services.AddValidatorsFromAssembly(ApplicationAssembly, ServiceLifetime.Scoped);
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