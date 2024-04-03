namespace Adnc.Demo.Usr.Application;

public sealed class DependencyRegistrar : AbstractApplicationDependencyRegistrar
{
    public override Assembly ApplicationLayerAssembly => Assembly.GetExecutingAssembly();

    public override Assembly ContractsLayerAssembly => typeof(IUserAppService).Assembly;

    public override Assembly RepositoryOrDomainLayerAssembly => typeof(EntityInfo).Assembly;

    public DependencyRegistrar(IServiceCollection services) : base(services)
    {
    }

    public override void AddAdnc()
    {
        base.AddAdnc();

        AddApplicaitonHostedServices();
        AddDbContextWithRepositories();
        AddMongoContextWithRepositries();
        AddRedisCaching();
        AddBloomFilters();
    }

    protected override void AddDbContextWithRepositories()
        => Services.AddMySqlDbContextAndRepository(MysqlSection, RepositoryOrDomainLayerAssembly);

    public override void AddAdncInfraMapper()
        => Services.AddAdncInfraAutoMapper(ApplicationLayerAssembly);  //使用AutoMapper 
}
