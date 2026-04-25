using RollPunk.AccessPolicy;
using RollPunk.HierarchyFields;
using RollPunk.Players;

namespace RollPunk.UIFields
{
    public class UIFieldAccessChecker
    {
        private EntityFieldsOwnersRegistry _ownersRegistry;

        public UIFieldAccessChecker(EntityFieldsOwnersRegistry ownersRegistry)
        {
            _ownersRegistry = ownersRegistry;
        }
        
        public bool IsFieldVisibleToPlayer(LineField field, Player player)
        {
            PlayerRole role = field.ViewAccessLevel;

            EntityField entity = field.GetEntityAncestor();

            if (entity == null)
                return PlayerRole.All >= role;

            PlayerRole relativeRole = _ownersRegistry.GetRelativePlayerRole(entity, player);
            return relativeRole >= role;
        }

        public bool IsFieldEditableByPlayer(LineField field, Player player)
        {
            PlayerRole role = field.EditAccessLevel;

            EntityField entity = field.GetEntityAncestor();

            if (entity == null)
                return PlayerRole.All >= role;

            PlayerRole relativeRole = _ownersRegistry.GetRelativePlayerRole(entity, player);
            return relativeRole >= role;
        }
    }
}
