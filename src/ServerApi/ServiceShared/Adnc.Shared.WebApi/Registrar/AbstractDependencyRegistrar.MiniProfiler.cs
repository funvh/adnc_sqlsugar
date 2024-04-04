namespace Adnc.Shared.WebApi.Registrar;

public abstract partial class AbstractDependencyRegistrar
{
    /// <summary>
    /// 注册 MiniProfiler 组件
    /// </summary>
    protected virtual void AddMiniProfiler() =>
        Services
        .AddMiniProfiler(options => options.RouteBasePath = $"/{ServiceInfo.RelativeRootPath}/profiler")
        .AddEntityFramework();
}
