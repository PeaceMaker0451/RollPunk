
using System;
using System.Reflection;
using System.Text;
using RollPunk.LuaApiGenerator.Reflection;

namespace RollPunk.LuaApiGenerator.Generation;

public sealed class EmmyLuaGenerator
{
    private readonly HashSet<Type> _apiTypes;
    private readonly HashSet<Type> _types;

    private readonly LuaTypeFormatter _typeFormatter = new();

    private readonly StringBuilder _builder = new();

    public EmmyLuaGenerator(
        IEnumerable<Type> apiTypes,
        HashSet<Type> types)
    {
        _apiTypes = apiTypes.ToHashSet();
        _types = types;
    }

    public string Generate()
    {
        _builder.AppendLine("---@meta");
        _builder.AppendLine();
        _builder.AppendLine("-- Generated automatically. Do not edit.");
        _builder.AppendLine();

        GenerateAliases();

        foreach (var type in _types.OrderBy(type => type.FullName))
            GenerateType(type);

        return _builder.ToString();
    }

    private void GenerateAliases()
    {
        _builder.AppendLine("---@alias Guid string");
        _builder.AppendLine();
    }

    private void GenerateType(Type type)
    {
        if (type.IsEnum)
        {
            GenerateEnum(type);
            return;
        }

        var name = ApiTypeScanner.GetTypeName(type);

        var genericParameters = type.IsGenericTypeDefinition
            ? $"<{string.Join(", ", type.GetGenericArguments().Select(a => a.Name))}>"
            : "";

        var baseType = type.BaseType;

        var inheritance = baseType != null &&
                          baseType != typeof(object) &&
                          _types.Contains(
                              baseType.IsGenericType
                                  ? baseType.GetGenericTypeDefinition()
                                  : baseType)
            ? $" : {_typeFormatter.Format(baseType)}"
            : "";

        _builder.AppendLine(
            $"---@class {name}{genericParameters}{inheritance}");

        foreach (var property in type.GetVisibleProperties())
        {
            _builder.AppendLine(
                $"---@field {property.Name} {_typeFormatter.Format(property.PropertyType)}");
        }

        foreach (var field in type.GetVisibleFields())
        {
            _builder.AppendLine(
                $"---@field {field.Name} {_typeFormatter.Format(field.FieldType)}");
        }

        // API objects are assumed to be globally available in Lua.
        // The base API class itself is only a type declaration.
        if (_apiTypes.Contains(type) && name != "API")
            _builder.AppendLine($"{name} = {{}}");

        _builder.AppendLine();

        if (_apiTypes.Contains(type))
            GenerateApiMethods(type);
    }

    private void GenerateEnum(Type type)
    {
        var name = ApiTypeScanner.GetTypeName(type);

        _builder.AppendLine($"---@enum {name}");
        _builder.AppendLine($"{name} = {{");

        foreach (var enumName in Enum.GetNames(type))
        {
            var value = Convert.ToInt64(
                Enum.Parse(type, enumName));

            _builder.AppendLine($"    {enumName} = {value},");
        }

        _builder.AppendLine("}");
        _builder.AppendLine();
    }

    private void GenerateApiMethods(Type type)
    {
        var methods = type.GetVisibleMethods()
            .GroupBy(method => method.Name)
            .OrderBy(group => group.Key);

        foreach (var group in methods)
        {
            var overloads = group
                .OrderBy(method => method.GetParameters().Length)
                .ToArray();

            var primary = overloads[0];

            // Additional overloads.
            foreach (var overload in overloads.Skip(1))
            {
                _builder.AppendLine(
                    $"---@overload fun({GetParameters(overload)})" +
                    GetReturnAnnotation(overload));
            }

            // Primary overload.
            GenerateParameterAnnotations(primary.GetParameters());

            if (primary.ReturnType != typeof(void))
            {
                _builder.AppendLine(
                    $"---@return {_typeFormatter.Format(primary.ReturnType)}");
            }

            var parameters = GetLuaParameters(primary.GetParameters());

            var typeName = ApiTypeScanner.GetTypeName(type);

            _builder.AppendLine(
                $"function {typeName}.{primary.Name}({parameters}) end");

            _builder.AppendLine();
        }
    }

    private void GenerateParameterAnnotations(ParameterInfo[] parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                var elementType = parameter.ParameterType.GetElementType()!;

                _builder.AppendLine(
                    $"---@vararg {_typeFormatter.Format(elementType)}");

                continue;
            }

            var name = GetParameterName(parameter);

            var optional = parameter.IsOptional || parameter.HasDefaultValue;

            _builder.AppendLine(
                $"---@param {name}{(optional ? "?" : "")} {_typeFormatter.Format(parameter.ParameterType)}");
        }
    }

    private string GetLuaParameters(ParameterInfo[] parameters)
    {
        var result = parameters
            .Where(parameter =>
                parameter.GetCustomAttribute<ParamArrayAttribute>() == null)
            .Select(GetParameterName)
            .ToList();

        if (parameters.Any(parameter =>
                parameter.GetCustomAttribute<ParamArrayAttribute>() != null))
        {
            result.Add("...");
        }

        return string.Join(", ", result);
    }

    private string GetParameters(MethodInfo method)
    {
        var parameters = method.GetParameters();

        var result = parameters
            .Where(parameter =>
                parameter.GetCustomAttribute<ParamArrayAttribute>() == null)
            .Select(parameter =>
            {
                var name = GetParameterName(parameter);
                var optional = parameter.IsOptional || parameter.HasDefaultValue;

                return $"{name}{(optional ? "?" : "")}: {_typeFormatter.Format(parameter.ParameterType)}";
            })
            .ToList();

        var paramsParameter = parameters.FirstOrDefault(parameter =>
            parameter.GetCustomAttribute<ParamArrayAttribute>() != null);

        if (paramsParameter != null)
        {
            result.Add(
                $"...: {_typeFormatter.Format(paramsParameter.ParameterType.GetElementType()!)}");
        }

        return string.Join(", ", result);
    }

    private string GetReturnAnnotation(MethodInfo method)
    {
        return method.ReturnType == typeof(void)
            ? ""
            : $": {_typeFormatter.Format(method.ReturnType)}";
    }

    private static string GetParameterName(ParameterInfo parameter)
    {
        return string.IsNullOrWhiteSpace(parameter.Name)
            ? $"arg{parameter.Position}"
            : parameter.Name;
    }
}
