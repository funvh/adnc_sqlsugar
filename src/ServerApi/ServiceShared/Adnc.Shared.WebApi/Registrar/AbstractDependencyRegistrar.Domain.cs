namespace Adnc.Shared.WebApi.Registrar;

public abstract partial class AbstractDependencyRegistrar
{
    /// <summary>
    /// 注册Domain服务
    /// </summary>
    protected virtual void AddDomainSerivces<TDomainService>(Action<IServiceCollection>? action = null)
        where TDomainService : class
    {
        action?.Invoke(Services);

        var serviceType = typeof(TDomainService);
        var implTypes = RepositoryOrDomainAssembly.ExportedTypes.Where(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true)).ToList();
        implTypes.ForEach(implType =>
        {
            Services.AddScoped(implType, implType);
        });
    }
}
