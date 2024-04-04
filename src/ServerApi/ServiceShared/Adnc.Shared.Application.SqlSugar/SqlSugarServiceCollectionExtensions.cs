using Adnc.Infra.Helper;
using Adnc.Infra.IdGenerater.Yitter;
using Adnc.Infra.IRepository;
using Adnc.Infra.IRepository.Entities;
using Adnc.Infra.IRepository.SqlSugar;
using Adnc.Infra.IRepository.SqlSugar.Abstracts;
using Adnc.Infra.IRepository.SqlSugar.Entities;
using Adnc.Infra.Repository.SqlSugar.Repositories;
using Adnc.Infra.Repository.SqlSugar.SqlServer;
using Adnc.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SqlSugar;
using System.Reflection;
using Yitter.IdGenerator;

namespace Microsoft.Extensions.DependencyInjection;

public static class SqlSugarServiceCollectionExtensions
{
    /// <summary>
    /// 添加SqlSugar服务与仓储
    /// </summary>
    public static void AddSqlSugarWithRepositories(this IServiceCollection services,
        IConfiguration configuration,
        Assembly repositoryAssmbly)
    {
        var connectionStringsSection = configuration.GetSection("ConnectionStrings");

        ArgumentNullException.ThrowIfNull(nameof(connectionStringsSection));

        if (connectionStringsSection.GetChildren().IsNullOrEmpty())
            throw new ArgumentNullException("数据库连接字符串未配置，请先配置数据库连接字符串");

        List<ConnectionConfig> configs = new List<ConnectionConfig>();
        foreach (var connectionString in connectionStringsSection.GetChildren())
        {
            configs.Add(new ConnectionConfig
            {
                ConfigId = connectionString.Key,
                ConnectionString = connectionString.Value,
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {
                    SqlServerCodeFirstNvarchar = true,
                },
                LanguageType = LanguageType.Chinese,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //注意:  这儿AOP设置不能少
                    EntityService = (c, p) =>
                    {
                        #region 设置自动可空类型

                        /***高版C#写法***/
                        //支持string?和string  
                        if (p.IsPrimarykey == false && new NullabilityInfoContext().Create(c).WriteState is NullabilityState.Nullable)
                        {
                            p.IsNullable = true;
                        }

                        #endregion
                    }
                }
            });
        }

        var sugarClient = new SqlSugarScope(configs, db =>
        {
            configs.ForEach(config =>
            {
                var dbProvider = db.GetConnectionScope(config.ConfigId);
                SetDbAop(dbProvider);
            });
        });

        var isCodeFirstSection = configuration.GetSection("DatabaseConfig:IsCodeFirst");
        if (isCodeFirstSection != null && bool.TryParse(isCodeFirstSection.Value, out bool isCodeFirst) && isCodeFirst)
        {
            sugarClient.CodeFirst.InitTables(typeof(SqlSugarEventTracker));
            InitDatabase(sugarClient, repositoryAssmbly);
        }

        var isInitDataSection = configuration.GetSection("DatabaseConfig:IsInitData");
        if (isInitDataSection != null && bool.TryParse(isInitDataSection.Value, out bool isInitData) && isInitData)
        {
            configs.ForEach(config =>
            {
                InitDataSeed(sugarClient, config, repositoryAssmbly);
            });
        }

        //配置使用第三方雪花算法生成Id
        StaticConfig.CustomSnowFlakeFunc = () => IdGenerater.GetNextId();

        services.AddSingleton<ISqlSugarClient>(sugarClient);

        services.TryAddScoped<IUnitOfWork, SqlSugarSqlServerUnitOfWork>();
        services.TryAddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
        services.TryAddScoped(typeof(ISqlSugarBasicRepository<>), typeof(SqlSugarBasicRepository<>));
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="db"></param>
    /// <param name="config"></param>
    private static void InitDatabase(SqlSugarScope db,
        Assembly repositoryAssmbly)
    {
        //这里创建操作日志表
        var sugarProvider = db.GetConnection(db.CurrentConnectionConfig.ConfigId);

        if (sugarProvider.CurrentConnectionConfig.DbType != SqlSugar.DbType.Oracle)
            sugarProvider.DbMaintenance.CreateDatabase();

        Type[] types = repositoryAssmbly.GetTypes()
            .Where(w => w.IsAssignableTo(typeof(SqlSugarEntity)) && w.IsNotAbstractClass(true))
            .ToArray();

        //根据types创建表
        sugarProvider.CodeFirst.SetStringDefaultLength(int.MaxValue).InitTables(types);
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="db"></param>
    /// <param name="config"></param>
    private static void InitDataSeed(SqlSugarScope db, ConnectionConfig config, Assembly assembly)
    {
        SqlSugarScopeProvider dbProvider = db.GetConnectionScope(config.ConfigId);

        // 获取所有种子配置-初始化数据
        var seedDataTypes = assembly.GetTypes().Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
            && u.HasImplementedRawGeneric(typeof(SqlSugarEntitySeedDataAbstract<>))).ToList();

        if (!seedDataTypes.Any())
            return;

        foreach (var seedType in seedDataTypes)
        {
            #region Old

            //var instance = Activator.CreateInstance(seedType);

            //var hasDataMethod = seedType.GetMethod("HasData");

            //var seedData = ((IEnumerable)hasDataMethod?.Invoke(instance, null))?.Cast<object>();
            //if (seedData == null) continue;

            //var entityType = seedType.GetInterfaces().First().GetGenericArguments().First();
            //var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(entityType);
            //if (entityInfo.Columns.Any(u => u.IsPrimarykey))
            //{
            //    // 按主键进行批量增加和更新
            //    var storage = dbProvider.StorageableByObject(seedData.ToList()).ToStorage();
            //    storage.AsInsertable.ExecuteCommand();
            //    storage.AsUpdateable.ExecuteCommand();
            //}
            //else
            //{
            //    // 无主键则只进行插入
            //    if (!dbProvider.Queryable(entityInfo.DbTableName, entityInfo.DbTableName).Any())
            //        dbProvider.InsertableByObject(seedData.ToList()).ExecuteCommand();
            //}

            #endregion

            #region New

            var instance = Activator.CreateInstance(seedType);

            var updateDataMethod = seedType.GetMethod("UpdateData");

            var result = updateDataMethod?.Invoke(instance, new[] { dbProvider.CopyNew() });

            #endregion
        }
    }

    /// <summary>
    /// 配置Aop
    /// </summary>
    /// <param name="db"></param>
    private static void SetDbAop(SqlSugarScopeProvider db)
    {
        var config = db.CurrentConnectionConfig;

        // 设置超时时间
        db.Ado.CommandTimeOut = 30;

        // 开发环境打印日志
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").EqualsIgnoreCase("Development"))
        {
            // 打印SQL语句
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                var originColor = Console.ForegroundColor;
                if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Green;
                if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Yellow;
                if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("【" + DateTime.Now + "——执行SQL】\r\n" + UtilMethods.GetSqlString(config.DbType, sql, pars) + "\r\n");
                Console.ForegroundColor = originColor;
                //App.PrintToMiniProfiler("SqlSugar", "Info", sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            };
            db.Aop.OnError = ex =>
            {
                if (ex.Parametres == null) return;
                var originColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                var pars = db.Utilities.SerializeObject(((SugarParameter[])ex.Parametres).ToDictionary(it => it.ParameterName, it => it.Value));
                Console.WriteLine("【" + DateTime.Now + "——错误SQL】\r\n" + UtilMethods.GetSqlString(config.DbType, ex.Sql, (SugarParameter[])ex.Parametres) + "\r\n");

                Console.Write(ex.StackTrace);
                //App.PrintToMiniProfiler("SqlSugar", "Error", $"{ex.Message}{Environment.NewLine}{ex.Sql}{pars}{Environment.NewLine}");
            };
        }

        long tenantId = 0;
        long userId = 0;
        // 配置租户过滤器
        var httpContext = InfraHelper.Accessor.GetCurrentHttpContext();
        if (httpContext is not null)
        {
            var tenantIdStr = httpContext.User?.FindFirst(JwtClaimNames.TenantId)?.Value;
            if (!string.IsNullOrWhiteSpace(tenantIdStr))
                long.TryParse(tenantIdStr, out tenantId);

            var userIdStr = httpContext.User?.FindFirst(JwtClaimNames.UserId)?.Value;
            if (!string.IsNullOrWhiteSpace(userIdStr))
                long.TryParse(userIdStr, out userId);
        }

        // 配置实体假删除过滤器
        db.QueryFilter.AddTableFilter<ISoftDeleteFilter>(it => it.IsDeleted == false);
        // 配置租户过滤器
        if (tenantId != 0)
            db.QueryFilter.AddTableFilter<ITenantIdFiter>(u => u.TenantId == tenantId);

        // 数据审计
        db.Aop.DataExecuting = (oldValue, entityInfo) =>
        {
            if (entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                // 主键(long类型)且没有值的---赋值雪花Id
                if (entityInfo.EntityColumnInfo.IsPrimarykey &&
                    entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                {
                    var id = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);
                    if (id == null || (long)id == 0)
                        entityInfo.SetValue(YitIdHelper.NextId());
                }

                if (tenantId != 0)
                {
                    if (entityInfo.PropertyName == "TenantId")
                    {
                        var entityTenantId = ((dynamic)entityInfo.EntityValue).TenantId;
                        if (entityTenantId == null || entityTenantId == 0)
                            entityInfo.SetValue(tenantId);
                    }
                }

                if (userId != 0)
                {
                    if (entityInfo.PropertyName == "Creator")
                    {
                        var creator = ((dynamic)entityInfo.EntityValue).Creator;
                        if (creator == 0 || creator == null)
                            entityInfo.SetValue(userId);
                    }
                }
            }
        };
    }
}
