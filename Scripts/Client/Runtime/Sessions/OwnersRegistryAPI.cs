using RollPunk.Debug;
using RollPunk.HierarchyFields;
using RollPunk.Modding.APIs;
using RollPunk.Players;
using System;

namespace RollPunk.AccessPolicy
{
    public class OwnersRegistryAPI : API
    {
        private EntityFieldsOwnersRegistry _registry;
        
        public OwnersRegistryAPI(EntityFieldsOwnersRegistry registry)
        {
            _registry = registry;
        }

        public void addEntityOwner(EntityFieldAPI? entity, PlayerAPI? player)
        {
            RPDebug.Log($"gegegege");
            if (_registry == null)
                throw new Exception("Чево балять?!?");
            
            if (entity == null)
                throw new NullReferenceException(nameof(entity));

            if (player == null)
                throw new NullReferenceException(nameof(player));

            _registry.AddEntityOwner((EntityField)entity.GetField(), player.GetPlayer());
        }

        public void removeEntityOwner(EntityFieldAPI? entity, PlayerAPI? player)
        {
            _registry.RemoveEntityOwner((EntityField)entity.GetField(), player.GetPlayer());
        }
    }
}
