using Adnc.Infra.Mapper;
using Adnc.Infra.Mapper.AutoMapper;
using Adnc.Infra.Mapper.Mapster;
using Mapster;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddAdncInfraAutoMapper(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
    {
        if (services.HasRegistered(nameof(AddAdncInfraAutoMapper)))
            return services;

        services.AddAutoMapper(profileAssemblyMarkerTypes);
        services.AddSingleton<IObjectMapper, AutoMapperObject>();
        return services;
    }

    public static IServiceCollection AddAdncInfraAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (services.HasRegistered(nameof(AddAdncInfraAutoMapper)))
            return services;

        services.AddAutoMapper(assemblies);
        services.AddSingleton<IObjectMapper, AutoMapperObject>();
        return services;
    }

    public static IServiceCollection AddAdncInfraMapster(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
    {
        if (services.HasRegistered(nameof(AddAdncInfraMapster)))
            return services;

        TypeAdapterConfig.GlobalSettings.Scan(profileAssemblyMarkerTypes.Select(s => s.Assembly).ToArray());

        services.AddSingleton<IObjectMapper, MapsterObject>();
        return services;
    }

    public static IServiceCollection AddAdncInfraMapster(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (services.HasRegistered(nameof(AddAdncInfraMapster)))
            return services;

        TypeAdapterConfig.GlobalSettings.Scan(assemblies);

        TypeAdapterConfig.GlobalSettings.AllowImplicitSourceInheritance = true;

        ////让只读属性也能映射
        //TypeAdapterConfig.GlobalSettings.Default
        //    .UseDestinationValue(member => member.SetterModifier == AccessModifier.None &&
        //        member.Type.IsGenericType &&
        //        member.Type.GetGenericTypeDefinition() == typeof(RepeatedField<>));

        services.AddSingleton<IObjectMapper, MapsterObject>();
        return services;
    }
}