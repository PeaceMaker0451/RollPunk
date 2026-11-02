using System.Reflection;

namespace RollPunk.LuaApiGenerator.Reflection;

public static class ApiTypeScanner
{
    private const string ApiSuffix = "API";
    private const string ApiBaseName = "API";

    public static Type[] FindApiTypes(IEnumerable<Assembly> assemblies)
    {
        return assemblies
            .SelectMany(AssemblyScanner.GetLoadableTypes)
            .Where(type => type.IsClass && IsApiType(type))
            .Distinct()
            .OrderBy(type => type.FullName)
            .ToArray();
    }

    public static bool IsApiType(Type type)
    {
        if (!type.IsClass)
            return false;

        // Inheritance from API.
        for (var current = type;
             current != null;
             current = current.BaseType)
        {
            if (current.Name == ApiBaseName)
                return true;
        }

        // API suffix.
        return type.Name.EndsWith(
            ApiSuffix,
            StringComparison.Ordinal);

        return false;
    }

    public static HashSet<Type> CollectTypes(IEnumerable<Type> apiTypes)
    {
        var result = new HashSet<Type>();
        var queue = new Queue<Type>(apiTypes);

        while (queue.Count > 0)
        {
            var type = queue.Dequeue();

            if (type == null || IsIgnoredType(type) || !result.Add(type))
                continue;

            // Base class.
            if (type.BaseType != null &&
                type.BaseType != typeof(object))
            {
                EnqueueType(type.BaseType, queue);
            }

            // Properties.
            foreach (var property in type.GetVisibleProperties())
                EnqueueType(property.PropertyType, queue);

            // Fields.
            foreach (var field in type.GetVisibleFields())
                EnqueueType(field.FieldType, queue);

            // Methods are collected only from API classes.
            if (!IsApiType(type))
                continue;

            foreach (var method in type.GetVisibleMethods())
            {
                EnqueueType(method.ReturnType, queue);

                foreach (var parameter in method.GetParameters())
                    EnqueueType(parameter.ParameterType, queue);
            }
        }

        return result;
    }

    private static void EnqueueType(Type type, Queue<Type> queue)
    {
        if (type.IsByRef || type.IsPointer || type.IsArray)
        {
            EnqueueType(type.GetElementType()!, queue);
            return;
        }

        if (type.IsGenericParameter)
            return;

        // Nullable<T>.
        var nullable = Nullable.GetUnderlyingType(type);

        if (nullable != null)
        {
            EnqueueType(nullable, queue);
            return;
        }

        // Generic types.
        if (type.IsGenericType)
        {
            foreach (var argument in type.GetGenericArguments())
                EnqueueType(argument, queue);

            var definition = type.GetGenericTypeDefinition();

            if (!IsIgnoredType(definition))
                queue.Enqueue(definition);

            return;
        }

        if (!IsIgnoredType(type))
            queue.Enqueue(type);
    }

    private static bool IsIgnoredType(Type type)
    {
        if (type.IsGenericParameter)
            return true;

        if (type.IsPrimitive ||
            type == typeof(string) ||
            type == typeof(object) ||
            type == typeof(decimal) ||
            type == typeof(Guid) ||
            type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(TimeSpan))
        {
            return true;
        }

        if (type.Namespace?.StartsWith(
                "System",
                StringComparison.Ordinal) == true)
        {
            return true;
        }

        if (type.FullName == "MoonSharp.Interpreter.DynValue")
            return true;

        return false;
    }

    public static void ValidateTypeNames(IEnumerable<Type> types)
    {
        var duplicates = types
            .GroupBy(type => GetTypeName(type))
            .Where(group =>
                group.Select(type => type.FullName).Distinct().Count() > 1)
            .ToArray();

        if (duplicates.Length == 0)
            return;

        throw new InvalidOperationException(
            "Types with conflicting Lua names: " +
            string.Join(", ", duplicates.Select(group => group.Key)));
    }

    public static string GetTypeName(Type type)
    {
        return type.Name.Split('`')[0];
    }
}