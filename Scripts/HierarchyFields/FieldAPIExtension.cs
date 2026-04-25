using RollPunk.Fields;
using RollPunk.Modding.APIs;

namespace RollPunk.HierarchyFields
{
    [APIExtension]
    public static class FieldAPIExtension
    {
        public static EntityFieldAPI? getEntityAncestor(this FieldAPI field)
        {
            return field.GetField().GetEntityAncestor()?.GetAPI() as EntityFieldAPI;
        }
    }
}
