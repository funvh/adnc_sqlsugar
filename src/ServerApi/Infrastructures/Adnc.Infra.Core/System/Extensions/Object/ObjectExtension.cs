namespace System;

public static class ObjectExtension
{
    /// <summary>
    /// 判断类型是否实现某个泛型
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="generic">泛型类型</param>
    /// <returns>bool</returns>
    public static bool HasImplementedRawGeneric(this Type type, Type generic)
    {
        // 检查接口类型
        var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
        if (isTheRawGenericType) return true;

        // 检查类型
        while (type != null && type != typeof(object))
        {
            isTheRawGenericType = IsTheRawGenericType(type);
            if (isTheRawGenericType) return true;
            type = type.BaseType;
        }

        return false;

        // 判断逻辑
        bool IsTheRawGenericType(Type type) => generic == (type.IsGenericType ? type.GetGenericTypeDefinition() : type);
    }

    /// <summary>
    ///  Converts the object to string or return an empty string if the value is null.
    /// </summary>
    /// <param name="obj">The @this to act on.</param>
    /// <returns>@this as a string or empty if the value is null.</returns>
    public static string ToSafeString(this object? obj)
    {
        if (obj is null)
            return string.Empty;

        return obj.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Remove leading and trailing spaces from all string fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public static void TrimStringFields<T>(this T obj) where T : class
    {
        if (obj is null) return;
        var stringProperties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                        .Where(p => p.PropertyType == typeof(string));

        foreach (var property in stringProperties)
        {
            var propertyValue = (string?)property.GetValue(obj);
            if (propertyValue != null)
            {
                var trimmedValue = propertyValue.Trim();
                property.SetValue(obj, trimmedValue);
            }
        }
    }
}