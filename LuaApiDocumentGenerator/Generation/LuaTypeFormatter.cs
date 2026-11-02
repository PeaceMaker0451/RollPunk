using RollPunk.LuaApiGenerator.Reflection;

namespace RollPunk.LuaApiGenerator.Generation;

public sealed class LuaTypeFormatter
{
    public string Format(Type type)
    {
        
        if (type.IsByRef || type.IsPointer)
            type = type.GetElementType()!;

        if (type == typeof(void))
            return "nil";

        if (type.FullName == "MoonSharp.Interpreter.DynValue")
            return "any";

        if (type == typeof(object))
            return "any";

        if (type == typeof(string) || type == typeof(char))
            return "string";

        if (type == typeof(bool))
            return "boolean";

        if (type == typeof(Guid))
            return "Guid";

        if (type == typeof(DateTime) ||
            type == typeof(DateTimeOffset))
            return "string";

        if (type.IsPrimitive || type == typeof(decimal))
            return "number";

        if (type.IsArray)
            return $"{Format(type.GetElementType()!)}[]";

        var nullable = Nullable.GetUnderlyingType(type);

        if (nullable != null)
            return $"{Format(nullable)}|nil";

        if (type.IsGenericParameter)
            return type.Name;

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();
            var arguments = type.GetGenericArguments();

            var name = definition.Name.Split('`')[0];

            if (name is "List" or "IList" or
                "IReadOnlyList" or "IEnumerable" or
                "ICollection" or "IReadOnlyCollection")
            {
                return $"{Format(arguments[0])}[]";
            }

            if (name is "Dictionary" or "IDictionary" or
                "IReadOnlyDictionary")
            {
                return $"table<{Format(arguments[0])}, {Format(arguments[1])}>";
            }

            return $"{ApiTypeScanner.GetTypeName(definition)}" +
                   $"<{string.Join(", ", arguments.Select(Format))}>";
        }

        return ApiTypeScanner.GetTypeName(type);
    }
}