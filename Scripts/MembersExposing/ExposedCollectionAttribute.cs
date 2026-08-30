namespace RollPunk.MembersExposing
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class ExposedCollectionAttribute : Attribute
    {
        public bool ReadOnly { get; init; }

        public string? DisplayName { get; init; }
    }
}
