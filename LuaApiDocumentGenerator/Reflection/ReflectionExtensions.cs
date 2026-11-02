using System.Reflection;

namespace RollPunk.LuaApiGenerator.Reflection;

public static class ReflectionExtensions
{
    public static bool IsMoonSharpHidden(this MemberInfo member)
    {
        return member.GetCustomAttributesData().Any(attribute =>
            attribute.AttributeType.Name == "MoonSharpHiddenAttribute");
    }

    public static PropertyInfo[] GetVisibleProperties(this Type type)
    {
        return type.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Where(property =>
            {
                if (property.IsMoonSharpHidden())
                    return false;

                if (property.GetIndexParameters().Length > 0)
                    return false;

                var accessors = new[]
                {
                    property.GetMethod,
                    property.SetMethod
                };

                var publicAccessors = accessors
                    .Where(method => method?.IsPublic == true)
                    .ToArray();

                return publicAccessors.Length > 0 &&
                       publicAccessors.All(method =>
                           !method!.IsMoonSharpHidden());
            })
            .ToArray();
    }

    public static MethodInfo[] GetVisibleMethods(this Type type)
    {
        return type.GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Where(method =>
                !method.IsSpecialName &&
                !method.IsMoonSharpHidden() &&
                !method.ContainsGenericParameters)
            .ToArray();
    }

    public static FieldInfo[] GetVisibleFields(this Type type)
    {
        return type.GetFields(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Where(field => !field.IsMoonSharpHidden())
            .ToArray();
    }
}