using Mapster;

namespace Adnc.Infra.Mapper.Mapster
{
    public class MapsterObject : IObjectMapper
    {
        public TDestination Map<TDestination>(object source)
        {
            return source.Adapt<TDestination>();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            return source.Adapt<TDestination>();
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            return source.Adapt(destination, TypeAdapterConfig.GlobalSettings);
        }
    }
}
