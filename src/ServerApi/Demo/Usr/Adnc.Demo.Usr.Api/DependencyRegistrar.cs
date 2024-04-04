using Adnc.Demo.Usr.Api.Authentication;
using Adnc.Demo.Usr.Api.Authorization;
using Adnc.Demo.Usr.Repository;
using Adnc.Shared.Application.Contracts.Interfaces;
using Adnc.Shared.Application.Services.Trackers;
using Adnc.Shared.WebApi.Registrar;
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

    protected override Assembly ApplicationContractAssembly => Assembly.Load(ServiceInfo.ApplicationContractAssemblyName);

    protected override Assembly ApplicationAssembly => Assembly.Load(ServiceInfo.ApplicationAssemblyName);

    protected override Assembly RepositoryOrDomainAssembly => typeof(EntityInfo).Assembly;

    public override void AddAdnc()
    {
        AddWebApiDefault<BearerAuthenticationLocalProcessor, PermissionLocalHandler>();
        AddHealthChecks(true, true, true, false);
        Services.AddGrpc();
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