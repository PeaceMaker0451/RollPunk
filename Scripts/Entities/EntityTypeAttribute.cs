namespace RollPunk.Entities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class EntityTypeAttribute : Attribute
    {
        public string Id { get; }

        public EntityTypeAttribute(string id)
        {
            Id = id;
        }
    }
}
