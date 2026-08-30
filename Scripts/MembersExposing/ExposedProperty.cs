namespace RollPunk.MembersExposing
{
    public sealed class ExposedProperty : ExposedMember
    {
        private readonly Func<object, object?> _getter;
        private readonly Action<object, object?>? _setter;

        public ExposedProperty(
            string name,
            string displayName,
            Type valueType,
            bool readOnly,
            Func<object, object?> getter,
            Action<object, object?>? setter)
            : base(
                name,
                displayName,
                valueType,
                ExposedMemberKind.Property,
                readOnly)
        {
            _getter = getter;
            _setter = setter;
        }

        public override object? GetValue(object target)
        {
            return _getter(target);
        }

        public override void SetValue(object target, object? value)
        {
            if (ReadOnly || _setter == null)
                throw new InvalidOperationException(
                    $"Property '{Name}' is read-only.");

            _setter(target, value);
        }
    }
}
