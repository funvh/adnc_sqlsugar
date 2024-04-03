using Adnc.Shared.Application.Services.Trackers;

namespace Adnc.Shared.Application.Registrar;

public abstract partial class AbstractApplicationDependencyRegistrar : IDependencyRegistrar
{
    public string Name => "application";
    public abstract Assembly ApplicationLayerAssembly { get; }
    public abstract Assembly ContractsLayerAssembly { get; }
    public abstract Assembly RepositoryOrDomainLayerAssembly { get; }
    protected SkyApmExtensions SkyApm { get; init; }
    protected List<AddressNode> RpcAddressInfo { get; init; }
    protected IServiceCollection Services { get; init; }
    protected IConfiguration Configuration { get; init; }
    protected IServiceInfo ServiceInfo { get; init; }
    protected IConfigurationSection RedisSection { get; init; }
    protected IConfigurationSection CapSection { get; init; }
    protected IConfigurationSection CachingSection { get; init; }
    protected IConfigurationSection MysqlSection { get; init; }
    protected IConfigurationSection MongoDbSection { get; init; }
    protected IConfigurationSection ConsulSection { get; init; }
    protected IConfigurationSection RabbitMqSection { get; init; }
    protected bool PollyStrategyEnable { get; init; }

    protected AbstractApplicationDependencyRegistrar(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentException("IServiceCollection is null.");
        Configuration = services.GetConfiguration() ?? throw new ArgumentException("Configuration is null.");
        ServiceInfo = services.GetServiceInfo() ?? throw new ArgumentException("ServiceInfo is null.");
        RedisSection = Configuration.GetSection(NodeConsts.Redis);
        CachingSection = Configuration.GetSection(NodeConsts.Caching);
        MongoDbSection = Configuration.GetSection(NodeConsts.MongoDb);
        MysqlSection = Configuration.GetSection(NodeConsts.Mysql);
        ConsulSection = Configuration.GetSection(NodeConsts.Consul);
        RabbitMqSection = Configuration.GetSection(NodeConsts.RabbitMq);
        CapSection = Configuration.GetSection(NodeConsts.Cap);
        SkyApm = Services.AddSkyApmExtensions();
        RpcAddressInfo = Configuration.GetSection(NodeConsts.RpcAddressInfo).Get<List<AddressNode>>();
        PollyStrategyEnable = Configuration.GetValue("Polly:Enable", false);
    }

    /// <summary>
    /// 注册基础服务
    /// </summary>
    public virtual void AddAdnc()
    {
        Services.AddAdncInfraYitterIdGenerater(RedisSection);
        Services.AddAdncInfraConsul(ConsulSection);
        Services.AddValidatorsFromAssembly(ContractsLayerAssembly, ServiceLifetime.Scoped);

        AddApplicationSharedServices();
        AddAppliactionSerivcesWithInterceptors();
        AddApplicaitonHostedServices();
        AddDbContextWithRepositories();
        AddMongoContextWithRepositries();
        AddRedisCaching();
        AddBloomFilters();
    }

    /// <summary>
    /// 添加模型映射
    /// </summary>
    public abstract void AddAdncInfraMapper();

    /// <summary>
    /// 注册application.shared层服务
    /// </summary>
    protected void AddApplicationSharedServices()
    {
        Services.AddSingleton(typeof(Lazy<>));
        Services.AddScoped<UserContext>();
        Services.AddScoped<OperateLogInterceptor>();
        Services.AddScoped<OperateLogAsyncInterceptor>();
        Services.AddScoped<UowInterceptor>();
        Services.AddScoped<UowAsyncInterceptor>();
        Services.AddSingleton<IBloomFilter, NullBloomFilter>();
        Services.AddSingleton<BloomFilterFactory>();
        Services.AddHostedService<CachingHostedService>();
        Services.AddHostedService<BloomFilterHostedService>();
        Services.AddHostedService<Channels.ChannelConsumersHostedService>();
        Services.AddScoped<IMessageTracker, DbMessageTrackerService>();
        Services.AddScoped<IMessageTracker, RedisMessageTrackerService>();
        Services.AddScoped<MessageTrackerFactory>();
    }
}