using RollPunk.HierarchyFields;
using RollPunk.Players;
using RollPunk.Entities;

namespace RollPunk.AccessPolicy
{
    public class EntityFieldsOwnersRegistry
    {
        private OwnershipContainer _ownerships;
        private Dictionary<Guid, EntityOwnership> _entityFieldIdIndex = new();

        public EntityFieldsOwnersRegistry(OwnershipContainer ownerships)
        {
            _ownerships = ownerships;
            
            // Инициализируем индекс существующими данными
            foreach (var ownership in _ownerships.Objects)
                _entityFieldIdIndex[ownership.EntityFieldId] = ownership;

            // Подписываемся на изменения для поддержания индекса
            _ownerships.Added += OnOwnershipAdded;
            _ownerships.Removed += OnOwnershipRemoved;
        }

        public void AddEntityOwner(EntityField entity, Player player)
        {
            var ownership = EnsureOwnershipExists(entity);
            ownership.OwnerIds.Add(player.ID);
        }

        public bool IsOwneredByPlayer(EntityField entity, Player player)
        {
            var ownership = GetOwnership(entity.ID);
            if (ownership == null)
                throw new InvalidOperationException($"Entity {entity.Name} ({entity.ID}) owner record doesn't exist!");

            return ownership.OwnerIds.Contains(player.ID);
        }

        public void RemoveEntityOwner(EntityField entity, Player player)
        {
            var ownership = GetOwnership(entity.ID);
            if (ownership == null)
                throw new InvalidOperationException($"Entity {entity.Name} ({entity.ID}) owner record doesn't exist!");

            if (!ownership.OwnerIds.Contains(player.ID))
                throw new InvalidOperationException($"Entity {entity.Name} ({entity.ID}) is not ownered by player {player.Name} ({player.ID})");

            ownership.OwnerIds.Remove(player.ID);
        }

        public void AddEntityTeam(EntityField entity, Guid team)
        {
            var ownership = EnsureOwnershipExists(entity);
            ownership.TeamIds.Add(team);
        }

        public void RemoveEntityTeam(EntityField entity, Guid team)
        {
            var ownership = EnsureOwnershipExists(entity);
            ownership.TeamIds.Remove(team);
        }

        public PlayerRole GetRelativePlayerRole(EntityField entity, Player player)
        {
            var ownership = GetOwnership(entity.ID);
            if (ownership == null)
                return PlayerRole.All;

            if (player.IsAdmin)
                return PlayerRole.Admin;

            if (ownership.OwnerIds.Contains(player.ID))
                return PlayerRole.Owner;

            if (player.TeamId != null && ownership.TeamIds.Contains((Guid)player.TeamId))
                return PlayerRole.Team;

            return PlayerRole.All;
        }

        public EntityOwnership GetOwnership(Guid entityFieldId)
        {
            _entityFieldIdIndex.TryGetValue(entityFieldId, out var ownership);
            return ownership;
        }

        private EntityOwnership EnsureOwnershipExists(EntityField entity)
        {
            var ownership = GetOwnership(entity.ID);
            if (ownership == null)
            {
                ownership = new EntityOwnership(entity.ID, $"Ownership_{entity.Name}");
                _ownerships.Add(ownership);
            }
            return ownership;
        }

        private void OnOwnershipAdded(EntityOwnership ownership)
        {
            _entityFieldIdIndex[ownership.EntityFieldId] = ownership;
        }

        private void OnOwnershipRemoved(EntityOwnership ownership)
        {
            _entityFieldIdIndex.Remove(ownership.EntityFieldId);
        }
    }
}
