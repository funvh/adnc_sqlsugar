using Adnc.Infra.Redis.Caching.Interceptor.Castle;
using Adnc.Shared.Application.Contracts.Interfaces;
using Adnc.Shared.Application.Interceptors;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Adnc.Shared.WebApi.Registrar;

public abstract partial class AbstractDependencyRegistrar
{
    protected static List<Type> DefaultInterceptorTypes
        => new() { typeof(OperateLogInterceptor), typeof(CachingInterceptor), typeof(UowInterceptor) };

    /// <summary>
    /// 注册Application服务
    /// </summary>
    protected virtual void AddAppliactionSerivcesWithInterceptors(
        Action<IServiceCollection>? action = null)
    {
        action?.Invoke(Services);

        var appServiceType = typeof(IAppService);
        var serviceTypes = AppServiceInterfaceLayerAssembly.GetExportedTypes().Where(type => type.IsInterface && type.IsAssignableTo(appServiceType)).ToList();
        serviceTypes.ForEach(serviceType =>
        {
            var implType = ApplicationAssembly.ExportedTypes.FirstOrDefault(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true));
            if (implType is null)
                return;

            Services.AddScoped(implType);
            Services.TryAddSingleton(new ProxyGenerator());
            Services.AddScoped(serviceType, provider =>
            {
                var interfaceToProxy = serviceType;
                var target = provider.GetService(implType);
                var interceptors = DefaultInterceptorTypes.ConvertAll(interceptorType => provider.GetService(interceptorType) as IInterceptor).ToArray();
                var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
                var proxy = proxyGenerator.CreateInterfaceProxyWithTargetInterface(interfaceToProxy, target, interceptors);
                return proxy;
            });
        });
    }

    /// <summary>
    /// 注册Application的IHostedService服务
    /// </summary>
    protected virtual void AddApplicaitonHostedServices()
    {
        var serviceType = typeof(IHostedService);
        var implTypes = ApplicationAssembly.ExportedTypes.Where(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true)).ToList();
        implTypes.ForEach(implType =>
        {
            Services.AddSingleton(serviceType, implType);
        });
    }
}
