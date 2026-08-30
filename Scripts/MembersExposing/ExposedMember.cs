namespace RollPunk.MembersExposing
{
    public enum ExposedMemberKind
    {
        Property,
        Collection
    }

    public abstract class ExposedMember
    {
        public string Name { get; }
        public string DisplayName { get; }
        public Type ValueType { get; }
        public ExposedMemberKind Kind { get; }
        public bool ReadOnly { get; }

        protected ExposedMember(
            string name,
            string displayName,
            Type valueType,
            ExposedMemberKind kind,
            bool readOnly)
        {
            Name = name;
            DisplayName = displayName;
            ValueType = valueType;
            Kind = kind;
            ReadOnly = readOnly;
        }

        public abstract object? GetValue(object target);

        public virtual void SetValue(object target, object? value)
        {
            throw new InvalidOperationException(
                $"Member '{Name}' is read-only.");
        }
    }
}
