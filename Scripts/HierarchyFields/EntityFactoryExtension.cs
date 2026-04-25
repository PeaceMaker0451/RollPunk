using RollPunk.Entities;

namespace RollPunk.HierarchyFields
{
    public static class EntityFactoryExtension
    {
        public static void RegisterHierarchyFields(this EntityFactory factory)
        {
            factory.Register<EntityField>();
        }
    }
}
