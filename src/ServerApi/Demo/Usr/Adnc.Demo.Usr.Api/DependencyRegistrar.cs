using Adnc.Demo.Usr.Api.Authentication;
using Adnc.Demo.Usr.Api.Authorization;
using Adnc.Shared.Application.Contracts.Interfaces;
using Adnc.Shared.Application.Services.Trackers;
using Adnc.Shared.WebApi.Registrar;
using FluentValidation;
using System.Reflection;

namespace Adnc.Demo.Usr.Api;

public sealed class UsrWebApiDependencyRegistrar : AbstractDependencyRegistrar
{
    public UsrWebApiDependencyRegistrar(IServiceCollection services)
        : base(services)
    {
    }

    public UsrWebApiDependencyRegistrar(IApplicationBuilder app)
        : base(app)
    {
    }

    protected override Assembly AppServiceInterfaceLayerAssembly => Assembly.Load(ServiceInfo.ApplicationContractAssemblyName);

    protected override Assembly ApplicationAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly RepositoryOrDomainAssembly => Assembly.Load(ServiceInfo.RepositoryAssemblyName);

    public override void AddAdnc()
    {
        AddWebApiDefault<BearerAuthenticationLocalProcessor, PermissionLocalHandler>();
        AddHealthChecks(true, true, true, false);
        Services.AddGrpc();

        // 添加模型验证扫描服务
        Services.AddValidatorsFromAssembly(AppServiceInterfaceLayerAssembly, ServiceLifetime.Scoped);
    }

    public override void UseAdnc()
    {
        UseWebApiDefault(endpointRoute: endpoint =>
        {
            endpoint.MapGrpcService<Grpc.AuthGrpcServer>();
            endpoint.MapGrpcService<Grpc.UsrGrpcServer>();
        });
    }

    protected override void AddDbContextWithRepositories()
        => Services.AddSqlSugarWithRepositories(Configuration, RepositoryOrDomainAssembly);

    protected override void AddMessageTrackerService()
    {
        Services.AddScoped<IMessageTracker, SqlSugarMessageTrackerService>();
    }
}