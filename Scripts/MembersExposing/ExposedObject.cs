namespace RollPunk.MembersExposing
{
    public sealed class ExposedObject
    {
        public object Target { get; }

        public IReadOnlyList<ExposedMember> Members { get; }

        public ExposedObject(
            object target,
            IReadOnlyList<ExposedMember> members)
        {
            Target = target;
            Members = members;
        }
    }
}
