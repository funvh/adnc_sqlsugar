using Adnc.Infra.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class EFCoreServiceCollectionExtensions
{
    /// <summary>
    /// 注册EFCoreContext与仓储
    /// </summary>
    public static IServiceCollection AddEfCoreContextWithRepositories(this IServiceCollection services,
        Assembly repositoryOrDomainAssembly,
        IConfiguration configuration)
    {
        var serviceType = typeof(IEntityInfo);
        var implType = repositoryOrDomainAssembly.ExportedTypes.FirstOrDefault(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true));
        if (implType is null)
            throw new NotImplementedException(nameof(IEntityInfo));
        else
            services.AddScoped(serviceType, implType);

        services.AddEfCoreContext(configuration);

        return services;
    }

    /// <summary>
    /// 注册EFCoreContext
    /// </summary>
    private static IServiceCollection AddEfCoreContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceInfo = services.GetServiceInfo();

        var connectionStrings = configuration.GetSection("ConnectionStrings");
        if (connectionStrings == null || connectionStrings.GetChildren().IsNullOrEmpty())
            throw new ArgumentNullException("请先配置数据库连接字符串");

        var connectionString = connectionStrings.GetChildren().Any(a => a.Key.EqualsIgnoreCase("Default")) ?
            connectionStrings.GetChildren().FirstOrDefault(f => f.Key.EqualsIgnoreCase("Default"))!.Value :
            connectionStrings.GetChildren().FirstOrDefault()!.Value;

        var dbType = configuration.GetSection("DatabaseConfig:DbType")?.Value;
        dbType ??= "SqlServer";

        switch (dbType)
        {
            case "SqlServer":
                services.AddAdncInfraEfCoreSQLServer(connectionString);
                break;
            case "MySql":
                var serverVersion = new MariaDbServerVersion(new Version(10, 5, 4));
                services.AddAdncInfraEfCoreMySql(options =>
                {
                    options.UseLowerCaseNamingConvention();
                    options.UseMySql(connectionString, serverVersion, optionsBuilder =>
                    {
                        optionsBuilder.MinBatchSize(4)
                                                .MigrationsAssembly(serviceInfo.MigrationsAssemblyName)
                                                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });

                    //if (this.IsDevelopment())
                    //{
                    //    //options.AddInterceptors(new DefaultDbCommandInterceptor())
                    //    options.LogTo(Console.WriteLine, LogLevel.Information)
                    //                .EnableSensitiveDataLogging()
                    //                .EnableDetailedErrors();
                    //}
                    //替换默认查询sql生成器,如果通过mycat中间件实现读写分离需要替换默认SQL工厂。
                    //options.ReplaceService<IQuerySqlGeneratorFactory, AdncMySqlQuerySqlGeneratorFactory>();
                });
                break;
            default:
                break;
        }

        return services;
    }
}
