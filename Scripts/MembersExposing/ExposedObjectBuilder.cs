using System.Reflection;

namespace RollPunk.MembersExposing
{
    public static class ExposedObjectBuilder
    {
        public static ExposedObject Build(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var members = BuildMembers(target.GetType());

            return new ExposedObject(target, members);
        }

        public static IReadOnlyList<ExposedMember> BuildMembers(Type type)
        {
            var members = new List<ExposedMember>();

            foreach (var property in GetProperties(type))
            {
                var propertyAttribute =
                    property.GetCustomAttribute<ExposedPropertyAttribute>();

                if (propertyAttribute != null)
                {
                    members.Add(BuildProperty(
                        property,
                        propertyAttribute));

                    continue;
                }

                var collectionAttribute =
                    property.GetCustomAttribute<ExposedCollectionAttribute>();

                if (collectionAttribute != null)
                {
                    members.Add(BuildCollection(
                        property,
                        collectionAttribute));
                }
            }

            return members;
        }

        private static ExposedProperty BuildProperty(
            PropertyInfo property,
            ExposedPropertyAttribute attribute)
        {
            if (property.GetMethod == null)
                throw new InvalidOperationException(
                    $"Exposed property '{property.Name}' must have a getter.");

            var setter = FindSetter(property, attribute);

            bool readOnly =
                attribute.ReadOnly ||
                setter == null;

            var getter = CreateGetter(property);
            var setterDelegate = readOnly
                ? null
                : CreateSetter(setter!);

            return new ExposedProperty(
                name: property.Name,
                displayName: attribute.DisplayName ?? property.Name,
                valueType: property.PropertyType,
                readOnly: readOnly,
                getter: getter,
                setter: setterDelegate);
        }

        private static ExposedCollection BuildCollection(
            PropertyInfo property,
            ExposedCollectionAttribute attribute)
        {
            if (property.GetMethod == null)
                throw new InvalidOperationException(
                    $"Exposed collection '{property.Name}' must have a getter.");

            var elementType = GetCollectionElementType(property.PropertyType);

            if (elementType == null)
            {
                throw new InvalidOperationException(
                    $"Unable to determine element type of exposed collection " +
                    $"'{property.DeclaringType?.Name}.{property.Name}'.");
            }

            return new ExposedCollection(
                name: property.Name,
                displayName: attribute.DisplayName ?? property.Name,
                collectionType: property.PropertyType,
                elementType: elementType,
                readOnly: attribute.ReadOnly,
                getter: CreateGetter(property));
        }

        private static MethodInfo? FindSetter(
            PropertyInfo property,
            ExposedPropertyAttribute attribute)
        {
            if (attribute.Setter != null)
            {
                return property.DeclaringType?.GetMethod(
                    attribute.Setter,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);
            }

            string setterName = $"Set{property.Name}";

            return property.DeclaringType?.GetMethod(
                setterName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic,
                binder: null,
                types: new[] { property.PropertyType },
                modifiers: null);
        }

        private static Func<object, object?> CreateGetter(
            PropertyInfo property)
        {
            return target => property.GetValue(target);
        }

        private static Action<object, object?> CreateSetter(
            MethodInfo setter)
        {
            return (target, value) =>
            {
                setter.Invoke(target, new[] { value });
            };
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public);
        }

        private static Type? GetCollectionElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();

                if (genericDefinition == typeof(IEnumerable<>) ||
                    genericDefinition == typeof(IReadOnlyList<>) ||
                    genericDefinition == typeof(IList<>) ||
                    genericDefinition == typeof(List<>))
                {
                    return type.GetGenericArguments()[0];
                }
            }

            var enumerableInterface = type
                .GetInterfaces()
                .FirstOrDefault(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableInterface?.GetGenericArguments()[0];
        }
    }
}
