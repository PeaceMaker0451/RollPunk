using RollPunk.HierarchyFields;
using RollPunk.Modding.APIs;
using RollPunk.Players;

namespace RollPunk.AccessPolicy
{
    public class OwnersRegistryAPI : API
    {
        private EntityFieldsOwnersRegistry _registry;
        public OwnersRegistryAPI(EntityFieldsOwnersRegistry registry)
        {
            _registry = registry;
        }

        public void setEntityOwner(EntityFieldAPI entity, PlayerAPI player)
        {
            _registry.AddEntityOwner((EntityField)entity.GetField(), player.GetPlayer());
        }

        public void removeEntityOwner(EntityFieldAPI entity, PlayerAPI player)
        {
            _registry.RemoveEntityOwner((EntityField)entity.GetField(), player.GetPlayer());
        }
    }
}
