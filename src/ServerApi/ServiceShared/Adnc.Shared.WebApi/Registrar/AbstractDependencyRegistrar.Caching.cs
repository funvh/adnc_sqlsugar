using Adnc.Shared.Application.BloomFilter;
using Adnc.Shared.Application.Caching;
using Adnc.Shared.Application.Caching.SkyApm;
using SkyApm;

namespace Adnc.Shared.WebApi.Registrar;

public abstract partial class AbstractDependencyRegistrar
{
    /// <summary>
    /// 注册Caching相关处理服务
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void AddRedisCaching(
        Action<IServiceCollection>? action = null)
    {
        action?.Invoke(Services);
        if (this.IsEnableSkyApm())
        {
            SkyApm.AddRedisCaching();
        }
        Services.AddAdncInfraRedisCaching(RedisSection, CachingSection);
        var serviceType = typeof(ICachePreheatable);
        var implTypes = ApplicationAssembly.ExportedTypes.Where(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true));
        if (implTypes.IsNotNullOrEmpty())
        {
            implTypes.ForEach(implType =>
            {
                Services.AddSingleton(implType, implType);
                Services.AddSingleton(x => (ICachePreheatable)x.GetRequiredService(implType));
            });
        }
    }

    /// <summary>
    /// 注册BloomFilter相关处理服务
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void AddBloomFilters(
        Action<IServiceCollection>? action = null)
    {
        action?.Invoke(Services);

        var serviceType = typeof(IBloomFilter);
        var implTypes = ApplicationAssembly.ExportedTypes.Where(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true)).ToList();

        if (implTypes.IsNotNullOrEmpty())
            implTypes.ForEach(implType => Services.AddSingleton(serviceType, implType));
    }
}
