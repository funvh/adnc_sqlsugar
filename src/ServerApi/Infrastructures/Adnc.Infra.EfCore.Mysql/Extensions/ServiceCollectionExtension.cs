using Adnc.Infra.Repository.EfCore.MySql;
using Adnc.Infra.Repository.EfCore.MySql.Configurations;
using Adnc.Infra.Repository.EfCore.MySql.Transaction;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddMySqlDbContextAndRepository(this IServiceCollection services,
        IConfigurationSection mysqlSection,
        Assembly repositoryAssembly)
    {
        var serviceType = typeof(EfEntity);
        var implType = repositoryAssembly.ExportedTypes.FirstOrDefault(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true));
        if (implType is null)
            throw new NotImplementedException(nameof(EfEntity));
        else
            services.AddScoped(serviceType, implType);

        var serviceInfo = services.GetServiceInfo();
        var mysqlConfig = mysqlSection.Get<MysqlOptions>();
        var serverVersion = new MariaDbServerVersion(new Version(10, 5, 4));
        services.AddAdncInfraEfCoreMySql(options =>
        {
            options.UseLowerCaseNamingConvention();
            options.UseMySql(mysqlConfig.ConnectionString, serverVersion, optionsBuilder =>
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

        return services;
    }

    public static IServiceCollection AddAdncInfraEfCoreMySql(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder)
    {
        if (services.HasRegistered(nameof(AddAdncInfraEfCoreMySql)))
            return services;

        services.TryAddScoped<IUnitOfWork, MySqlUnitOfWork<MySqlDbContext>>();
        services.TryAddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));
        services.TryAddScoped(typeof(IEfBasicRepository<>), typeof(EfBasicRepository<>));
        services.AddDbContext<DbContext, MySqlDbContext>(optionsBuilder);

        return services;
    }
}