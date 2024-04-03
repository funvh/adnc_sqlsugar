using Adnc.Infra.Repository.Mongo;
using Adnc.Infra.Repository.Mongo.Configuration;
using Adnc.Infra.Repository.Mongo.Extensions;

namespace Adnc.Shared.Application.Registrar;

public abstract partial class AbstractApplicationDependencyRegistrar
{
    /// <summary>
    /// 注册数据库DBContext和仓储（可以使用EFCore或者SQLSugar）
    /// </summary>
    protected abstract void AddDbContextWithRepositories();

    /// <summary>
    /// 注册MongoContext与仓储
    /// </summary>
    protected virtual void AddMongoContextWithRepositries(Action<IServiceCollection>? action = null)
    {
        action?.Invoke(Services);

        var mongoConfig = MongoDbSection.Get<MongoOptions>();
        Services.AddAdncInfraMongo<MongoContext>(options =>
        {
            options.ConnectionString = mongoConfig.ConnectionString;
            options.PluralizeCollectionNames = mongoConfig.PluralizeCollectionNames;
            options.CollectionNamingConvention = (NamingConvention)mongoConfig.CollectionNamingConvention;
        });
    }
}