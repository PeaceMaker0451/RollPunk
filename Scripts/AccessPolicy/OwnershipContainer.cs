using RollPunk.Entities;

namespace RollPunk.AccessPolicy
{
    public class OwnershipContainer
    {
        private Dictionary<Guid, EntityOwnership> _ownerships = new();

        public event Action<EntityOwnership> Added;
        public event Action<EntityOwnership> Removed;
        public event Action<EntityOwnership> Changed;

        public IReadOnlyList<EntityOwnership> Objects => _ownerships.Values.ToList();
        public IReadOnlyDictionary<Guid, EntityOwnership> Dictionary => _ownerships;

        public EntityOwnership GetByID(Guid id)
        {
            _ownerships.TryGetValue(id, out EntityOwnership ownership);
            return ownership;
        }

        public void Add(EntityOwnership ownership)
        {
            if (_ownerships.ContainsKey(ownership.ID))
                throw new InvalidOperationException($"Ownership with ID {ownership.ID} already exists");

            _ownerships.Add(ownership.ID, ownership);
            SubscribeToOwnershipChanges(ownership);
            Added?.Invoke(ownership);
        }

        public bool Remove(Guid id)
        {
            if (_ownerships.TryGetValue(id, out EntityOwnership ownership))
            {
                UnsubscribeFromOwnershipChanges(ownership);
                _ownerships.Remove(id);
                Removed?.Invoke(ownership);
                return true;
            }
            return false;
        }

        public bool Remove(EntityOwnership ownership)
        {
            return Remove(ownership.ID);
        }

        public void Clear()
        {
            var ownershipsList = _ownerships.Values.ToList();
            foreach (var ownership in ownershipsList)
                Remove(ownership);
        }

        private void SubscribeToOwnershipChanges(EntityOwnership ownership)
        {
            ownership.Changed += OnOwnershipChanged;
        }

        private void UnsubscribeFromOwnershipChanges(EntityOwnership ownership)
        {
            ownership.Changed -= OnOwnershipChanged;
        }

        private void OnOwnershipChanged(EntityOwnership ownership)
        {
            Changed?.Invoke(ownership);
        }

        public void UpdateFromState(Dictionary<Guid, EntityState> ownershipStates)
        {
            EntityUpdater updater = new();
            
            foreach (var ownershipState in ownershipStates)
            {
                var existingOwnership = GetByID(ownershipState.Key);
                if (existingOwnership != null)
                {
                    updater.UpdateEntity(existingOwnership, ownershipState.Value);
                }
                else
                {
                    Add(new EntityOwnership(ownershipState.Value));
                }
            }
        }

        public Dictionary<Guid, EntityState> GetStates()
        {
            var states = new Dictionary<Guid, EntityState>();
            foreach (var ownership in _ownerships.Values)
            {
                states.Add(ownership.ID, ownership.GetState());
            }
            return states;
        }
    }
}
