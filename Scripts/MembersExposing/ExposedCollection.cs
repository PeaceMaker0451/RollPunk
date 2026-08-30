using System.Collections;

namespace RollPunk.MembersExposing
{
    public sealed class ExposedCollection : ExposedMember
    {
        private readonly Func<object, object?> _getter;

        public Type ElementType { get; }

        public ExposedCollection(
            string name,
            string displayName,
            Type collectionType,
            Type elementType,
            bool readOnly,
            Func<object, object?> getter)
            : base(
                name,
                displayName,
                collectionType,
                ExposedMemberKind.Collection,
                readOnly)
        {
            _getter = getter;
            ElementType = elementType;
        }

        public override object? GetValue(object target)
        {
            return _getter(target);
        }

        public IEnumerable<object?> GetItems(object target)
        {
            var value = GetValue(target);

            if (value is not IEnumerable enumerable)
                throw new InvalidOperationException(
                    $"Exposed collection '{Name}' returned " +
                    $"{value?.GetType().Name ?? "null"}.");

            foreach (var item in enumerable)
                yield return item;
        }
    }
}
