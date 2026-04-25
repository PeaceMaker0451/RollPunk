using RollPunk.Fields;

namespace RollPunk.HierarchyFields
{
    public static class FieldExtension
    {
        public static EntityField GetEntityAncestor(this Field field)
        {
            if (field is EntityField)
                return field as EntityField;

            Field owner = field.Parent;

            if (owner == null)
                return null;
            else if (owner is EntityField entityField)
                return entityField;
            else
                return GetEntityAncestor(owner);
        }
    }
}
