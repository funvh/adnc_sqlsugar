using Adnc.Shared.Application.BloomFilter;
using Adnc.Shared.Application.Caching;
using Adnc.Shared.Application.Channels;
using Adnc.Shared.Application.Contracts.Interfaces;
using Adnc.Shared.Application.Interceptors;
using Adnc.Shared.Application.Services.Trackers;
using Adnc.Shared.WebApi.Authentication;
using Adnc.Shared.WebApi.Authorization;
using SkyApm.Utilities.DependencyInjection;

namespace Adnc.Shared.WebApi.Registrar;

public abstract partial class AbstractDependencyRegistrar : IDependencyRegistrar
{
    public string Name => "webapi";
    protected IConfiguration Configuration { get; init; } = default!;
    protected IServiceCollection Services { get; init; } = default!;
    protected IServiceInfo ServiceInfo { get; init; } = default!;

    /// <summary>
    /// 应用服务接口所在层
    /// </summary>
    protected abstract Assembly AppServiceInterfaceLayerAssembly { get; }

    /// <summary>
    /// 应用服务所在层
    /// </summary>
    protected abstract Assembly ApplicationAssembly { get; }

    /// <summary>
    /// 仓储或领域所在层
    /// </summary>
    protected abstract Assembly RepositoryOrDomainAssembly { get; }

    /// <summary>
    /// SkyApm扩展
    /// </summary>
    protected SkyApmExtensions SkyApm { get; init; }

    /// <summary>
    /// Redis配置节点
    /// </summary>
    protected IConfigurationSection RedisSection { get; init; }

    /// <summary>
    /// 缓存配置节点
    /// </summary>
    protected IConfigurationSection CachingSection { get; init; }

    /// <summary>
    /// Consul配置节点
    /// </summary>
    protected IConfigurationSection ConsulSection { get; init; }
    protected IConfigurationSection RabbitMqSection { get; init; }
    protected IConfigurationSection CapSection { get; init; }
    protected IConfigurationSection MongoDbSection { get; init; }
    protected List<AddressNode> RpcAddressInfo { get; init; }

    /// <summary>
    /// 服务注册与系统配置
    /// </summary>
    /// <param name="services"><see cref="IServiceInfo"/></param>
    protected AbstractDependencyRegistrar(IServiceCollection services)
    {
        Services = services;

        Configuration = services.GetConfiguration() ?? throw new ArgumentException("Configuration is null.");
        ServiceInfo = services.GetServiceInfo() ?? throw new ArgumentException("ServiceInfo is null.");

        RedisSection = Configuration.GetSection(NodeConsts.Redis);
        CachingSection = Configuration.GetSection(NodeConsts.Caching);
        MongoDbSection = Configuration.GetSection(NodeConsts.MongoDb);
        //MysqlSection = Configuration.GetSection(NodeConsts.Mysql);
        ConsulSection = Configuration.GetSection(NodeConsts.Consul);
        RabbitMqSection = Configuration.GetSection(NodeConsts.RabbitMq);
        CapSection = Configuration.GetSection(NodeConsts.Cap);
        SkyApm = Services.AddSkyApmExtensions();
        RpcAddressInfo = Configuration.GetSection(NodeConsts.RpcAddressInfo).Get<List<AddressNode>>();
        //PollyStrategyEnable = Configuration.GetValue("Polly:Enable", false);
    }

    /// <summary>
    /// 注册服务入口方法
    /// </summary>
    public abstract void AddAdnc();

    /// <summary>
    /// 注册Webapi通用的服务
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    protected virtual void AddWebApiDefault() =>
        AddWebApiDefault<BearerAuthenticationRemoteProcessor, PermissionRemoteHandler>();

    /// <summary>
    /// 注册Webapi通用的服务
    /// </summary>
    /// <typeparam name="TAuthenticationProcessor"><see cref="AbstractAuthenticationProcessor"/></typeparam>
    /// <typeparam name="TAuthorizationHandler"><see cref="AbstractPermissionHandler"/></typeparam>
    protected virtual void AddWebApiDefault<TAuthenticationProcessor, TAuthorizationHandler>()
        where TAuthenticationProcessor : AbstractAuthenticationProcessor
        where TAuthorizationHandler : AbstractPermissionHandler
    {
        Services
            .AddHttpContextAccessor()
            .AddMemoryCache();

        Configure();
        AddControllers();
        AddAuthentication<TAuthenticationProcessor>();
        AddAuthorization<TAuthorizationHandler>();
        AddCors();

        var enableSwaggerUI = Configuration.GetValue(NodeConsts.SwaggerUI_Enable, true);
        if (enableSwaggerUI)
        {
            AddSwaggerGen();
            AddMiniProfiler();
        }

        Services.AddAdncInfraYitterIdGenerater(RedisSection);

        Services.AddAdncInfraConsul(ConsulSection);

        AddApplicationSharedServices();

        AddAppliactionSerivcesWithInterceptors();

        AddApplicaitonHostedServices();

        AddDbContextWithRepositories();

        AddMongoContextWithRepositries();

        AddRedisCaching();

        AddBloomFilters();

        AddAdncInfraMapper();

        AddMessageTrackerService();
    }

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
        Services.AddHostedService<ChannelConsumersHostedService>();

        Services.AddScoped<IMessageTracker, RedisMessageTrackerService>();
        Services.AddScoped<MessageTrackerFactory>();
    }

    /// <summary>
    /// 添加消息跟踪服务
    /// </summary>
    protected virtual void AddMessageTrackerService()
    {
        Services.AddScoped<IMessageTracker, DbMessageTrackerService>();
    }

    /// <summary>
    /// 添加模型映射
    /// </summary>
    protected virtual void AddAdncInfraMapper()
    {
        Services.AddAdncInfraAutoMapper(ApplicationAssembly);
    }
}