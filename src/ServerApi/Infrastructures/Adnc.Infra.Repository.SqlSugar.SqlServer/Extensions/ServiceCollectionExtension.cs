using Adnc.Infra.IRepository.SqlSugar;
using Adnc.Infra.Repository.SqlSugar.Repositories;
using Adnc.Infra.Repository.SqlSugar.SqlServer;
using SqlSugar;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 添加 SqlSugar 拓展
    /// </summary>
    /// <param name="services"></param>
    /// <param name="clients"></param>
    /// <returns></returns>
    public static IServiceCollection AddAdncInfraSqlSugarSqlServer(this IServiceCollection services,
        List<SqlSugarClient> clients)
    {
        if (services.HasRegistered(nameof(AddAdncInfraSqlSugarSqlServer)))
            return services;

        // 注册 SqlSugar 客户端
        foreach (var client in clients)
            services.AddScoped<ISqlSugarClient>(builder =>
            {
                return client;
            });

        services.TryAddScoped<IUnitOfWork, SqlSugarSqlServerUnitOfWork>();
        services.TryAddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
        services.TryAddScoped(typeof(ISqlSugarBasicRepository<>), typeof(SqlSugarBasicRepository<>));
        //services.AddDbContext<SugarUnitOfWork, SqlServerDbContext>();

        return services;
    }

    /// <summary>
    /// 添加 SqlSugar 拓展
    /// </summary>
    /// <param name="services"></param>
    /// <param name="clients"></param>
    /// <returns></returns>
    public static IServiceCollection AddAdncInfraSqlSugarSqlServer(this IServiceCollection services,
        List<SqlSugarScope> clients)
    {
        if (services.HasRegistered(nameof(AddAdncInfraSqlSugarSqlServer)))
            return services;

        // 注册 SqlSugar 客户端
        foreach (var client in clients)
            services.AddSingleton<ISqlSugarClient>(client);

        services.TryAddScoped<IUnitOfWork, SqlSugarSqlServerUnitOfWork>();
        services.TryAddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
        services.TryAddScoped(typeof(ISqlSugarBasicRepository<>), typeof(SqlSugarBasicRepository<>));
        //services.AddDbContext<SugarUnitOfWork, SqlServerDbContext>();

        return services;
    }

    public static IServiceCollection AddAdncInfraSqlSugarSqlServer(this IServiceCollection services, SqlSugarClient client)
    {
        return services.AddAdncInfraSqlSugarSqlServer(new List<SqlSugarClient> { client });
    }

    public static IServiceCollection AddAdncInfraSqlSugarSqlServer(this IServiceCollection services, SqlSugarScope client)
    {
        return services.AddAdncInfraSqlSugarSqlServer(new List<SqlSugarScope> { client });
    }
}