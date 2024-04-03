using Adnc.Demo.Maint.Repository;
using Adnc.Infra.Repository.EfCore.MySql.Configurations;
using Adnc.Shared.Rpc.Http.Services;
using Microsoft.Extensions.Configuration;
using IUsrRestClient = Adnc.Demo.Shared.Rpc.Http.Services.IUsrRestClient;

namespace Adnc.Demo.Maint.Application;

public sealed class DependencyRegistrar : AbstractApplicationDependencyRegistrar
{
    public override Assembly ApplicationLayerAssembly => Assembly.GetExecutingAssembly();

    public override Assembly ContractsLayerAssembly => Assembly.GetExecutingAssembly();

    public override Assembly RepositoryOrDomainLayerAssembly => typeof(EntityInfo).Assembly;

    public DependencyRegistrar(IServiceCollection services) : base(services)
    {
    }

    public override void AddAdnc()
    {
        base.AddAdnc();

        //rpc-rest
        var restPolicies = PollyStrategyEnable ? this.GenerateDefaultRefitPolicies() : new();
        AddRestClient<IAuthRestClient>(ServiceAddressConsts.AdncDemoAuthService, restPolicies);
        AddRestClient<IUsrRestClient>(ServiceAddressConsts.AdncDemoUsrService, restPolicies);

        AddRabbitMqClient();
    }

    public override void AddAdncInfraMapper()
        => Services.AddAutoMapper(ApplicationLayerAssembly);

    protected override void AddDbContextWithRepositories()
       => Services.AddMySqlDbContextAndRepository(MysqlSection, RepositoryOrDomainLayerAssembly);
}