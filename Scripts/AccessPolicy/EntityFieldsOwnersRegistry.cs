using RollPunk.HierarchyFields;
using RollPunk.Players;

namespace RollPunk.AccessPolicy
{
    public class EntityFieldsOwnersRegistry
    {
        private Dictionary<Guid, EntityOwnerRecord> _owners = new();

        public void AddEntityOwner(EntityField entity, Player player)
        {
            EnsureOwnerRecordExists(entity);

            _owners[entity.ID].OwnerIds.Add(player.ID);
        }

        public bool IsOwneredByPlayer(EntityField entity, Player player)
        {
            ThrowExceptionIfOwnerRecordNotExist(entity);
            var record = _owners[entity.ID];

            return record.OwnerIds.Contains(player.ID);
        }

        public void RemoveEntityOwner(EntityField entity, Player player)
        {
            ThrowExceptionIfOwnerRecordNotExist(entity);
            var record = _owners[entity.ID];

            if (IsOwneredByPlayer(entity, player) == false)
                throw new InvalidOperationException($"Entity {entity.Name} ({entity.ID}) is not ownered by player {player.Name} ({player.ID})");

            record.OwnerIds.Remove(player.ID);
        }

        public void AddEntityTeam(EntityField entity, Guid team)
        {
            EnsureOwnerRecordExists(entity);
            _owners[entity.ID].TeamIds.Add(team);
        }

        public void RemoveEntityTeam(EntityField entity, Guid team)
        {
            EnsureOwnerRecordExists(entity);
            _owners[entity.ID].TeamIds.Remove(team);
        }

        public PlayerRole GetRelativePlayerRole(EntityField entity, Player player)
        {
            EnsureOwnerRecordExists(entity);

            if (player.IsAdmin)
                return PlayerRole.Admin;

            if (_owners[entity.ID].OwnerIds.Contains(player.ID))
                return PlayerRole.Owner;

            if (player.TeamId != null && _owners[entity.ID].TeamIds.Contains((Guid)player.TeamId))
                return PlayerRole.Team;

            return PlayerRole.All;
        }

        private void EnsureOwnerRecordExists(EntityField entity)
        {
            if (_owners.ContainsKey(entity.ID) == false)
                _owners.Add(entity.ID, new EntityOwnerRecord());
        }

        private void ThrowExceptionIfOwnerRecordNotExist(EntityField entity)
        {
            if (_owners.ContainsKey(entity.ID) == false)
                throw new InvalidOperationException($"Entity {entity.Name} ({entity.ID}) owner record doesn't exist!");
        }

        internal class EntityOwnerRecord
        {
            public HashSet<Guid> OwnerIds { get; } = new();
            public HashSet<Guid> TeamIds { get; } = new();
        }
    }
}
